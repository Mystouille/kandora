namespace kandora.bot.resources
{
    class Resources
    {
        public const string cultureInfoStr = "fr-FR";

        //Admin commands
        public const string admin_groupDescription = "Commandes d'administation";
        public const string admin_startLeague_description = "Commence une ligue de mahjong";
        public const string admin_startLeague_leagueStarted = "Une ligue de mahjong vient de commencer sur {0}!";
        public const string admin_startLeague_leagueAlreadyExists = "Une ligue existe déjà sur {0}!";
        public const string admin_endLeague_description = "Supprime une ligue de mahjong ainsi que l'historique des parties";
        public const string admin_endLeague_leagueEnded = "La ligue de mahjong a été supprimée!";
        public const string admin_testLeague_description = "Supprime la ligue actuelle et en crée une composée de 4 joueurs";
        public const string admin_testLeague_testLeagueStarted = "Une ligue de test a été créée avec ces joueurs:\n{0}";
        //Quizz commands
        public const string quizz_groupDescription = "Commence un quizz";
        public const string quizz_fullflush_description = "Commence un quizz de main pure";
        public const string quizz_options_suit = "famille";
        public const string quizz_options_suit_description = "Famille de tuiles à utiliser [Optionel]";
        public const string quizz_options_questions = "nbQuestions";
        public const string quizz_options_questions_description = "Le nombre de questions voulu [Optionel]";
        public const string quizz_option_value_random = "aléatoire";
        public const string quizz_option_timeout = "timeout";
        public const string quizz_option_timeout_description = "Nombre de secondes au bout desquelles la question expire (0 = mort subite) [Optionel]";

        //Mahjong commands
        public const string mahjong_groupDescription = "Commandes liées au mahjong";
        public const string mahjong_image_description = "Affiche une main de mahjong (texte => image)";
        public const string mahjong_nanikiru_description = "Affiche une main de 14 tuiles et demande de choisir un écart";
        public const string mahjong_option_handstr = "main";
        public const string mahjong_option_handstr_description = "Exemple: 12333s456p555m11z. Optionel: dragons= [RWG]d, vents= [ESWN]w";
        public const string mahjong_option_potentialDiscards = "defausses";
        public const string mahjong_option_potentialDiscards_description = "Défausses permises de la main. [Optionel]";
        public const string mahjong_option_doras = "doras";
        public const string mahjong_option_doras_description = "Doras. Exemple: 1p4s [Optionel]";
        public const string mahjong_option_seat = "joueur";
        public const string mahjong_option_seat_east = "Est";
        public const string mahjong_option_seat_south = "Sud";
        public const string mahjong_option_seat_west = "Ouest";
        public const string mahjong_option_seat_north = "Nord";
        public const string mahjong_option_seat_description = "Vent du joueur [Optionel]";
        public const string mahjong_option_round = "manche";
        public const string mahjong_option_round_description = "Manche actuelle. Exemple: S3 [Optionel]";
        public const string mahjong_option_turn = "tour";
        public const string mahjong_option_turn_description = "Tour dans la manche [Optionel]";
        public const string mahjong_option_thread = "fil";
        public const string mahjong_option_thread_description = "Crée un fil de discussion [Optionel]";
        public const string mahjong_nanikiru_seat = "Joueur {0}";
        public const string mahjong_nanikiru_round = "Pendant {0}";
        public const string mahjong_nanikiru_turn = "Tour {0}";
        public const string mahjong_nanikiru_dora = "Dora {0}";
        public const string mahjong_nanikiru_wwyd = "Que feriez vous?";
        public const string mahjong_nanikiru_discussHere = "Discutez-en ici!";


        //Ongoing Quizz
        // chinitsu
        public const string quizz_fullflush_threadName = "Quizz chinitsu agari du {0} ({1} manches)";
        public const string quizz_fullflush_startMessage = "Un quizz de main pure commence.\n{0} manches";
        public const string quizz_fullflush_questionMessageWithTime = "Question **[{0} / {1}]**. Trouvez les tuiles qui completent la main.\nVous avez {2} secondes.";
        public const string quizz_fullflush_questionMessage = "Question **[{0} / {1}]**. Trouvez les tuiles qui completent la main. Seul le premier gagne!";

        // general
        public const string quizz_timer_disclaimer = ", {0} secondes par question.";
        public const string quizz_generatingProblem = "Affichage dans 3... 2... 1...";
        public const string quizz_timeoutNoWinnerMessage = "C'était peut-être un peu trop dur?";
        public const string quizz_suddenDeathWinnerMessage = "{0} a trouvé en premier!";
        public const string quizz_answer = "La réponse est {0}";
        public const string quizz_isOver = "**Le quizz est terminé!**";
        public const string quizz_error_problemAlreadyExists = "Un quizz est déjà en cours sur ce serveur";

        //Command attributes
        public const string commandAttributeError_title = "Commande annulée";
        public const string commandAttributeError_permissionMessage = "Je n'ai pas les permissions nécessaires dans ce salon";
        public const string commandAttributeError_adminMessage = "Tu n'es pas owner du server ou admin du bot";

        //Slash commands
        public const string commandError_title = "Une erreur s'est produite";
    }
}
