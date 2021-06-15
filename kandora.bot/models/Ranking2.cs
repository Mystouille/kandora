using System;
using System.Linq;
using System.Collections.Generic;

namespace RiichiTest
{
    class Program
    {
        //readonly static float stddev = 300;
        readonly static float mc_sampling = 1000;

        public static List<int> compute_ranks<T>(List<T> l)
        {
            // Returns the rank of each element of the list (in descending order)
            // i.e. (54,19,8,34) => 0,2,3,1
            // "The element at index 3 (34) is the 2nd largest value"  
            int N = l.Count;
            var sorted = l.Select((x, i) => new KeyValuePair<T, int>(x, i)).OrderBy(x => x.Key).Reverse().ToList();
            List<int> r = new List<int>(new int[N]);
            for(int i=0; i< N;  i++)
            {
                r[sorted[i].Value] = i;
            }
            return r;
        }

        public static int round_hundreds(float n)
        {
            return ((int)Math.Round(n / 100f)) * 100;
        }


        public static float SampleGaussian(Random random, double mean, double stddev)
        {
            // Return a random value sampled from NORMAL(mean, stddev^2)
            // The method requires sampling from a uniform random of (0,1]
            // but Random.NextDouble() returns a sample of [0,1).
            double x1 = 1 - random.NextDouble();
            double x2 = 1 - random.NextDouble();

            double y1 = Math.Sqrt(-2.0 * Math.Log(x1)) * Math.Cos(2.0 * Math.PI * x2);
            return (float)(y1 * stddev + mean);
        }

        public static List<int> SimulateGame(List<int> elos, int starting_scores, List<int> oka, List<int> uma, double std=300 )
        {
            // Using the given elo values, simulate a game a returns the final scores (count oka and uma)
            // Simulation follows this procedure:
            //   - Sample an "actual" elo for each player using a normal distribution around the given elo.
            //   - Compute scores based on these values (score_i = sampled_elo_i / sum(sampled_elos))
            //   - Apply oka and uma.
            Random r = new Random(elos.Sum());
            int N = elos.Count;
            List<float> sampled_elos = new List<float>();
            for (int i = 0; i < N; i++)
            {
                sampled_elos.Add(SampleGaussian(r, elos[i], std));
            }
            List<int> sampled_scores = ComputeProRataScores(sampled_elos, starting_scores);
            return transform_scores(sampled_scores, starting_scores, oka, uma);
        }

        static List<int> ComputeProRataScores(List<float> sampled_elos, int starting_points)
        {
            // Given a list of sampled_elos and starting point, compute the final score of each player (before oka/uma)
            // The N * starting point in play are distributed with the pro rata.
            //float min_sampled_elo = 0;
            int N = sampled_elos.Count;
            float sum_elos = sampled_elos.Sum();

            List<int> scores = new List<int>();
            float sum_exp_elos = 0.0f;
            for (int i = 0; i < N; i++)
            {
                sum_exp_elos += (float) Math.Exp(sampled_elos[i]/1000f);
            }
            for (int i = 0; i < N; i++)
            {
                scores.Add(round_hundreds((float)Math.Exp(sampled_elos[i] / 1000f) * N * starting_points / sum_exp_elos));
            }
            scores = correct_sum_int(scores, N * starting_points);
            return scores;

           
            //for(int i = 0; i < N; i++)
            //{
            //    scores.Add(round_hundreds((min_sampled_elo+sampled_elos[i]) * N * starting_points / sum_elos));
            //}
            //// Because of rounding, we may need to adjust the values.
            //return correct_sum_int(scores, N * starting_points);
        }

        public static List<float> ComputeExpectedScores(List<int> elos, int starting_scores, List<int> oka, List<int> uma)
        {
            // Given a list of elos, compute the expected score of each player using Monte-Carlo simulations.
            Random r = new Random();
            int N = elos.Count;
            List<float> expected_score = new List<float>(new float[N]);
            for (int sample = 0; sample < Program.mc_sampling; sample++)
            {
                List<int> sampled_scores = SimulateGame(elos, starting_scores, oka, uma);
                for (int i=0; i<N; i++)
                {
                    expected_score[i] += sampled_scores[i];
                }
            }
            for (int i = 0; i < N; i++)
            {
                expected_score[i] /= Program.mc_sampling;
            }
            return expected_score;
        }

        

        static List<float> ClampVariation(List<float> values, float max_var)
        {
            // Rescale all the values so that the largest absolute value is below max_var
            int N = values.Count;
            float M = Math.Max(values.Max(), -values.Min());
            if (M > max_var)
            {
                float adjustment = max_var / M;
                for (int p = 0; p < N; p++)
                {
                    values[p] *= adjustment;
                }
            }
            return values;
        }

        public static List<int> correct_sum_int(List<int> l, int s)
        {
            int diff_sum = l.Sum() - s;
            int N = l.Count;
            List<int> order = compute_ranks<int>(l);
            if (diff_sum > 0)
            {
                l[order[0]] -= diff_sum;
            }
            else if (diff_sum < 0)
            {
                l[order[N - 1]] -= diff_sum;
            }
            return l;
        }

        static List<int> transform_scores(List<int> scores, int starting_scores, List<int> oka, List<int> uma)
        {
            // Applies uma, oka and all those to get the final scores from the raw scores.
            int N = scores.Count;
            List<int> players_order = compute_ranks(scores);
            List<int> final_scores = new List<int>();
            for (int i = 0; i < N; i++)
            {
                final_scores.Add((int)((scores[i] - starting_scores) / 1000 + oka[players_order[i]] + uma[players_order[i]]));
            }
            // Adjust if sum != 0
            return correct_sum_int(final_scores, 0);
        }

        static List<int> computeEloVariations(List<int> final_scores, List<int> elos, int starting_scores, List<int> oka, List<int> uma, float factor, float max_var)
        {
            // Given the finals scores of a game and the elos of the players, computes the variation to apply.
            int N = elos.Count;
            // Compute the final scores with uka and uma
            //List<int> final_scores = transform_scores(scores, starting_scores, oka, uma);
            // Compute the expected scores for these elos
            List<float> final_theoretical_scores = ComputeExpectedScores(elos, starting_scores, oka, uma);
            // Compute the difference between the two, and apply a factor
            List<float> elo_variation = new List<float>();
            for (int i = 0; i < N; i++)
            {
                elo_variation.Add(factor * (final_scores[i] - final_theoretical_scores[i]));
            }
            // Limit the variation to max_var
            elo_variation = ClampVariation(elo_variation, max_var);
            // Transform to int, making sure the sum is 0.
            List<int> elo_variation_int = new List<int>();
            for (int i = 0; i < N; i++)
            {
                elo_variation_int.Add((int)Math.Round(elo_variation[i]));
            }
            elo_variation_int = correct_sum_int(elo_variation_int, 0);

            return elo_variation_int;
        }


        static List<int> sample(int max, int n)
        {
            Random rnd = new Random();
            return Enumerable.Range(0, max).OrderBy(x => rnd.Next()).Take(n).ToList();
        }

        static void Main(string[] args)
        {
            // A quick simulation. We simulated games between 16 players to check that their elos converge to reasonnable values
            //float p = 0f;
            Console.WriteLine("Hello World!");

            List<int> true_elos = new List<int> { 
                1060, 1120, 1160, 1201,
                1250, 1299, 1248, 1300,
                1410, 1450, 1680, 1600,
                1800, 2000, 2170, 2252
            };
            List<int> elos = new List<int> {
                1500, 1500, 1500, 1500, 
                1500, 1500, 1500, 1500, 
                1500, 1500, 1500, 1500, 
                1500, 1500, 1500, 1500, 
            };
            List<int> oka = new List<int> { 15, -5, -5, -5 };
            List<int> uma = new List<int> { 15, 5, -5, -15 };
            for (int i = 0; i < 400; i++)
            {
                List<int> player_indexes = Program.sample(16, 4);
                List<int> iteration_true_elos = new List<int> {
                    true_elos[player_indexes[0]],
                    true_elos[player_indexes[1]],
                    true_elos[player_indexes[2]],
                    true_elos[player_indexes[3]],
                };
                List<int> iteration_elos = new List<int> {
                    elos[player_indexes[0]],
                    elos[player_indexes[1]],
                    elos[player_indexes[2]],
                    elos[player_indexes[3]],
                };
                List<int> elo_var = computeEloVariations(
                    Program.SimulateGame(iteration_true_elos, 25000, new List<int> { 15, -5, -5, -5 }, new List<int> { 15, 5, -5, -15 },300),
                    //Program.transform_scores(Program.ComputeProRataScores(new List<float> { 1100f, 1700f, 1200f, 2000f }, 25000), 25000, new List<int> { 15, -5, -5, -5 }, new List<int> { 15, 5, -5, -15 }),
                    iteration_elos,
                    25000,
                    oka,
                    uma,
                    1.0f - i*0.001f,
                    50f
                );
                float sum_diff = 0f;
                List<float> ratios = new List<float>();
                for (int j = 0; j < 4; j++)
                {
                    elos[player_indexes[j]] += elo_var[j];
                };
                for (int j = 0; j < 16; j++)
                {
                    sum_diff += Math.Abs(elos[j] - true_elos[j]);
                    ratios.Add((float)elos[j] / true_elos[j]);
                };
                //Console.WriteLine(string.Join(' ', elos));
                //Console.WriteLine(string.Join(' ',elo_var));
                Console.WriteLine(string.Join(' ', sum_diff / 16f));
                Console.WriteLine(string.Join(' ', elos));
                Console.WriteLine(string.Join(' ', true_elos));
                Console.WriteLine(string.Join(' ', ratios));
                Console.WriteLine(' ');
            }
            Console.WriteLine(elos.Sum()/16f);
            Console.WriteLine(true_elos.Sum());
        }
    }
}
