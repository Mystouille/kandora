namespace kandora.bot.resources
{
    class EnglishResources
    {
        public const string cultureInfoStr = "en-US";

        //Admin commands
        public const string admin_groupDescription = "Admin commands";
        public const string admin_startLeague_description = "Starts a mahjong league";
        public const string admin_startLeague_leagueStarted = "A mahjong league just started on {0}!";
        public const string admin_endLeague_description = "Delete the existing mahjong league and all existing users and games";
        public const string admin_endLeague_leagueEnded = "The mahjong league got deleted!";
        public const string admin_testLeague_description = "Delete the current mahjong league and create one with 4 existing players";
        public const string admin_testLeague_testLeagueStarted = "A test league got created with these players:\n{0}";
        //Quizz commands
        public const string quizz_groupDescription = "Starts a quizz";
        public const string quizz_fullflush_description = "Starts a \"full flush\" quizz";
        public const string quizz_options_suit = "suit";
        public const string quizz_options_suit_description = "Suit to display";
        public const string quizz_options_questions = "nbQuestions";
        public const string quizz_options_questions_description = "The number of questions";
        public const string quizz_option_value_random = "random";
        public const string quizz_option_timeout = "timeout";
        public const string quizz_option_timeout_description = "number of seconds after which a question expires (0 = sudden death)";

        //Mahjong commands
        public const string mahjong_groupDescription = "Mahjong related commands";
        public const string mahjong_image_description = "Displays a mahjong hand (text => image)";
        public const string mahjong_nanikiru_description = "Displays a 14 tiles hand and prompts for a discard";
        public const string mahjong_option_handstr = "hand";
        public const string mahjong_option_handstr_description = "Example: 12333s456p555m11z. Optional: dragons= [RWG]d, winds= [ESWN]w";
        public const string mahjong_option_potentialDiscards = "discards";
        public const string mahjong_option_potentialDiscards_description = "Allowed discards of the hand. [Optional]";
        public const string mahjong_option_doras = "doras";
        public const string mahjong_option_doras_description = "Doras. Example: 1p4s [Optional]";
        public const string mahjong_option_seat = "seat";
        public const string mahjong_option_seat_east = "East";
        public const string mahjong_option_seat_south = "South";
        public const string mahjong_option_seat_west = "West";
        public const string mahjong_option_seat_north = "North";
        public const string mahjong_option_seat_description = "Player's wind [Optional]";
        public const string mahjong_option_round = "round";
        public const string mahjong_option_round_description = "Current round. Example: S3 [Optional]";
        public const string mahjong_option_turn = "turn";
        public const string mahjong_option_turn_description = "Current turn [Optional]";
        public const string mahjong_option_thread = "thread";
        public const string mahjong_option_thread_description = "Create a thread about it [Optionel]";
        public const string mahjong_nanikiru_seat = "Player {0}";
        public const string mahjong_nanikiru_round = "During {0}";
        public const string mahjong_nanikiru_turn = "Turn {0}";
        public const string mahjong_nanikiru_dora = "Dora {0}";
        public const string mahjong_nanikiru_wwyd = "What would you cut?";
        public const string mahjong_nanikiru_discussHere = "Discuss it here!";

        //Ongoing Quizz
        // fullflush
        public const string quizz_fullflush_threadName = "Chinitsu agari quizz of {0} ({1} rounds)";
        public const string quizz_fullflush_startMessage = "A \"full flush\" quizz is starting.\n{0} rounds";
        public const string quizz_fullflush_questionMessageWithTime = "Question **[{0} / {1}]**. Find the tiles that complete this hand.\nYou have {2} seconds.";
        public const string quizz_fullflush_questionMessage = "Question **[{0} / {1}]**. Find the tiles that complete this hand. The first one wins!";

        // general
        public const string quizz_timer_disclaimer = ", {0} seconds per question.";
        public const string quizz_generatingProblem = "Displaying in 3... 2... 1...";
        public const string quizz_timeoutNoWinnerMessage = "Looks like this was a bit too hard for you!";
        public const string quizz_suddenDeathWinnerMessage = "{0} got this one first!";
        public const string quizz_answer = "The answer is {0}";
        public const string quizz_isOver = "**Quizz is over!**";
        public const string quizz_error_problemAlreadyExists = "A quizz is already ongoing on this server";

        //Command attributes
        public const string commandAttributeError_title = "Command cancelled";
        public const string commandAttributeError_permissionMessage = "I don't have the required permissions in this channel";
        public const string commandAttributeError_adminMessage = "You are not this server's owner or this bot's admin";

        //Slash commands
        public const string commandError_title = "An error happened";
    }
}
