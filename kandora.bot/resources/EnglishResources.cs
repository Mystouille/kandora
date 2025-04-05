using Newtonsoft.Json;

namespace kandora.bot.resources
{
    class EnglishResources
    {
        public const string cultureInfoStr = "en-US";

        //Admin commands
        public const string admin_groupDescription = "Admin commands";
        public const string admin_startLeaderboard_description = "Starts a mahjong leaderboard";
        public const string admin_startLeaderboard_leaderboardStarted = "A mahjong leaderboard has been created on {0}!";
        public const string admin_startLeaderboard_leaderboardAlreadyExists = "A leaderboard already exists on {0}!";
        public const string admin_startLeague_description = "Starts a mahjong team league";
        public const string admin_startLeague_tournamentId = "tournamentId";
        public const string admin_startLeague_tournamentId_description = "A Riichi City tournament ID";
        public const string admin_startLeague_displayName = "displayName";
        public const string admin_startLeague_displayName_description = "The league's display name";
        public const string admin_startLeague_leagueStarted = "A mahjong team league has been created on {0}!";
        public const string admin_startLeague_leagueAlreadyExists = "A league already exists on {0}!";
        public const string admin_endLeague_description = "Ends the ongoing league";
        public const string admin_endLeague_leagueEnded = "{0} just ended";
        public const string admin_editLeague_description = "Changes league parameters";
        public const string admin_editLeague_leagueChanged = "The current league has been modified";
        public const string admin_addSub_description = "Add a sub for a game in the current league";
        public const string admin_addBotSub_description = "Add a sub but a bot for a game in the current league";
        public const string admin_addSub_userOut = "subbedOutUser";
        public const string admin_addSub_userOut_description = "The user that is subbed out (@ mention or guest name)";
        public const string admin_addSub_userIn = "subbedInUser";
        public const string admin_addSub_userIn_description = "The user that is subbed in (@ mention or guest name)";
        public const string admin_addSub_botIn = "botId";
        public const string admin_addSub_botIn_description = "The id of the bot that is subbed in";
        public const string admin_addSub_gameId = "gameId";
        public const string admin_addSub_gameId_description = "The Riichi City game id (log id)";
        public const string admin_addSub_playerHasBeenSubbed = "Player {0} has been subbed by {1} for game id {2}";
        public const string admin_addBotSub_playerHasBeenSubbed = "Player {0} has been subbed by bot {1} for game id {2}";
        public const string admin_startLeague_cutoff = "finalsCutoff";
        public const string admin_startLeague_cutoff_description = "Start date of the finals phase (yyyy/MM/dd)";
        public const string admin_createTeam_description = "Create a team for the current league";
        public const string admin_createTeam_teamName = "teamName";
        public const string admin_createTeam_teamName_description = "The team's name (no spaces nor special characters)";
        public const string admin_createTeam_fancyName = "fancyName";
        public const string admin_createTeam_fancyName_description = "The team's display name (go wild on these emotes)";
        public const string admin_createTeam_teamCreated = "The team {0} has been created in {1}!";
        public const string admin_addTeamPlayer_description = "Add a player to a team";
        public const string admin_addTeamPlayer_user = "userName";
        public const string admin_addTeamPlayer_user_description = "The player's name or mention (@)";
        public const string admin_addTeamPlayer_team = "teamName";
        public const string admin_addTeamPlayer_team_description = "The team's name";
        public const string admin_addTeamPlayer_userHasBeenAdded = "The user {0} has been added to team {1}!";
        public const string admin_flushServer_description = "Delete everything on a server";
        public const string admin_flushServer_unauthorized = "This commands deletes every game and every player data, so let's not do that for now.";
        public const string admin_flushServer_leaderboardEnded = "The mahjong leaderboard got deleted!";
        public const string admin_testLeaderboard_description = "Delete the current mahjong leaderboard and create one with 4 existing players";
        public const string admin_testLeaderboard_testLeaderboardStarted = "A test leaderboard got created with these players:\n{0}";
        public const string admin_showConfig_description = "Display the leaderboard's or leaderboard current parameters";
        public const string admin_seeConfig_type_league = "Team league";
        public const string admin_seeConfig_type_leaderboard = "Leaderboard";
        public const string admin_seeConfig_type = "type";
        public const string admin_seeConfig_type_description = "The desired config";
        public const string admin_setLeaderboardConfig_description = "Configure the leaderboard";
        public const string admin_setLeaderboardConfig_allowSanma = "allowSanma";
        public const string admin_setLeaderboardConfig_allowSanma_description = "Allow 3 player games";
        public const string admin_setLeaderboardConfig_countPoints = "countPoints";
        public const string admin_setLeaderboardConfig_eloSystem_None = "None";
        public const string admin_setLeaderboardConfig_eloSystem_Average = "Average";
        public const string admin_setLeaderboardConfig_eloSystem_Simple = "Simple";
        public const string admin_setLeaderboardConfig_eloSystem_Full = "Full";
        public const string admin_setLeaderboardConfig_countPoints_description = "Take players points into account";
        public const string admin_setLeaderboardConfig_eloSystem = "eloSystem";
        public const string admin_setLeaderboardConfig_eloSystem_description = "Select the ELO system";
        public const string admin_setLeaderboardConfig_startTime = "startDate";
        public const string admin_setLeaderboardConfig_startTime_description = "Leaderboard's start date (format: YYYY/MM/DD)";
        public const string admin_setLeaderboardConfig_endTime = "endDate";
        public const string admin_setLeaderboardConfig_endTime_description = "Leaderboard's end date (format: YYYY/MM/DD)";
        public const string admin_setLeaderboardConfig_startingPoints = "startingPoints";
        public const string admin_setLeaderboardConfig_startingPoints_description = "Player's starting points (in thousands)";
        public const string admin_setLeaderboardConfig_uma3p1 = "uma3p1";
        public const string admin_setLeaderboardConfig_uma3p1_description = "First player's uma (if sanma and points are activated)";
        public const string admin_setLeaderboardConfig_uma3p2 = "uma3p2";
        public const string admin_setLeaderboardConfig_uma3p2_description = "Second player's uma (if sanma and points are activated)";
        public const string admin_setLeaderboardConfig_uma3p3 = "uma3p3";
        public const string admin_setLeaderboardConfig_uma3p3_description = "Last player's uma (if sanma and points are activated)";
        public const string admin_setLeaderboardConfig_uma4p1 = "uma4p1";
        public const string admin_setLeaderboardConfig_uma4p1_description = "First player's uma (if points are activated)";
        public const string admin_setLeaderboardConfig_uma4p2 = "uma4p2";
        public const string admin_setLeaderboardConfig_uma4p2_description = "Second player's uma (if points are activated)";
        public const string admin_setLeaderboardConfig_uma4p3 = "uma4p3";
        public const string admin_setLeaderboardConfig_uma4p3_description = "Third player's uma (if points are activated)";
        public const string admin_setLeaderboardConfig_uma4p4 = "uma4p4";
        public const string admin_setLeaderboardConfig_uma4p4_description = "Last player's uma (if points are activated)";
        public const string admin_setLeaderboardConfig_oka = "oka";
        public const string admin_setLeaderboardConfig_oka_description = "Oka (payment to the first player, if points are activated)";
        public const string admin_setLeaderboardConfig_penaltyLast = "penaltyLast";
        public const string admin_setLeaderboardConfig_penaltyLast_description = "Last place penalty (if points are activated)";
        public const string admin_setLeaderboardConfig_penaltyChombo = "penaltyChombo";
        public const string admin_setLeaderboardConfig_penaltyChombo_description = "Penalty per chombo";
        public const string admin_setLeaderboardConfig_initialElo = "initialElo";
        public const string admin_setLeaderboardConfig_initialElo_description = "Initial ELO (if ELO is activated)";
        public const string admin_setLeaderboardConfig_minElo = "minElo";
        public const string admin_setLeaderboardConfig_minElo_description = "Minimum ELO (if ELO is activated)";
        public const string admin_setLeaderboardConfig_eloChangeDampening = "eloChangeDampening";
        public const string admin_setLeaderboardConfig_eloChangeDampening_description = "ELO change dampening (if ELO is activated)";
        public const string admin_setLeaderboardConfig_eloChangeStartRatio = "eloChangeStartRatio";
        public const string admin_setLeaderboardConfig_eloChangeStartRatio_description = "ELO/Points change dampening at first game (if ELO or points are activated)";
        public const string admin_setLeaderboardConfig_eloChangeEndRatio = "eloChangeEndRatio";
        public const string admin_setLeaderboardConfig_eloChangeEndRatio_description = "ELO/Points change dampening after trial period (if ELO or points are activated)";
        public const string admin_setLeaderboardConfig_trialPeriodDuration = "trialPeriodDuration";
        public const string admin_setLeaderboardConfig_trialPeriodDuration_description = "Number of game during which the ELO/points dampening changes (if ELO or points are activated)";
        public const string admin_setLeaderboardConfig_backfillInProgress = "The leaderboard config has been changed.\n{0}\n\n :bangbang: Kandora is recomputing the rankings... ";
        public const string admin_setLeaderboardConfig_backfillFinished = "The leaderboard config has been changed.\n{0}\n\n :white_check_mark: Kandora has finished recomputing the rankings! ";

        public const string admin_addPlayer_description = "Register a user to the server's leaderboard";
        public const string admin_addPlayer_nickname = "name";
        public const string admin_addPlayer_nickname_description = "The user's mention (with @) or his name if he's not on discord";
        public const string admin_addPlayer_Success = "The player {0} has been added to the leaderboard";
        public const string admin_setUser_description = "Change a user's info (use with caution)";
        public const string admin_setUser_Success = "The user {0} was edited";

        public const string admin_migratePlayer_description = "Change a user's name, and migrate all his game history";
        public const string admin_migratePlayer_sourceName = "currentName";
        public const string admin_migratePlayer_sourceName_description = "His current name (in case of a guest user) or his mention (with @)";
        public const string admin_migratePlayer_targetName = "targetName";
        public const string admin_migratePlayer_targetName_description = "His new name or a mention of a user in the server";

        //League commands
        public const string league_groupDescription = "League commands";
        public const string admin_startGame_description = "Start a league game";
        public const string admin_startGame_user1 = "user1";
        public const string admin_startGame_user1_description = "The player's name or mention (@)";
        public const string admin_startGame_user2 = "user2";
        public const string admin_startGame_user2_description = "The player's name or mention (@)";
        public const string admin_startGame_user3 = "user3";
        public const string admin_startGame_user3_description = "The player's name or mention (@)";
        public const string admin_startGame_user4 = "user4";
        public const string admin_startGame_user4_description = "The player's name or mention (@)";
        public const string admin_startGame_gameStarted = "The game has started!";
        public const string admin_seeLeagueRanking_description = "Displays the ranking of the ongoing league";
        public const string admin_seeLeagueRanking_displayInChat = "displayInChat";
        public const string admin_seeLeagueRanking_displayInChat_description = "Displays the ranking publicly";
        public const string admin_seeLeagueDelta_description = "Displays the combined results of the last N games.";
        public const string admin_seeLeagueDelta_numberOfGames = "numberOfGames";
        public const string admin_seeLeagueDelta_numberOfGames_description = "If spêcified, shows the delta over the last N games";


        //Leaderboard commands
        public const string leaderboard_groupDescription = "Leaderboard commands";
        public const string leaderboard_register_description = "Register to the leaderboard or change your info";
        public const string leaderboard_register_mahjsoulName = "mahjsoulname";
        public const string leaderboard_register_mahjsoulName_description = "Your Mahjong Soul nickname";
        public const string leaderboard_register_mahjsoulFriendId = "mahjsoulid";
        public const string leaderboard_register_mahjsoulFriendId_description = "Your Mahjong Soul ID";
        public const string leaderboard_register_tenhouName = "tenhouname";
        public const string leaderboard_register_tenhouName_description = "Your Tenhou nickname";
        public const string leaderboard_register_riichiCityId = "riichicityid";
        public const string leaderboard_register_riichiCityId_description = "The Riichi City user ID";
        public const string leaderboard_register_riichiCityName = "riichicityname";
        public const string leaderboard_register_riichiCityName_description = "The Riichi City nickname";
        public const string leaderboard_register_response_newUser = "You are now registered to this server's leaderboard";
        public const string leaderboard_register_response_newRanking = "You are now registered to this server's leaderboard. Your tenhou and mahjsoul names are shared between other servers leaderboards";
        public const string leaderboard_register_response_userAlreadyRegistered = "Your new info has been recorded";

        public const string leaderboard_logInfo_description = "Display a game's info from its ID";
        public const string leaderboard_option_gameId = "id";
        public const string leaderboard_option_gameId_description = "The game's ID (tenhou or mahjsoul)";
        public const string leaderboard_title = "Title";
        public const string leaderboard_date = "Date";
        public const string leaderboard_results = "Results";
        public const string leaderboard_bestHand = "Best hand: {0}({1}) (round {2}) with {3}, total:{4}";
        
        public const string leaderboard_getGames_description = "Download a csv file containing the games played during this current leaderboard";
        public const string leaderboard_getGames_fileHeader = "logId,gameName,location,date,player1,score1,chombo1,player2,score2,chombo2,player3,score3,chombo3,player4,score4,chombo4";
        public const string leaderboard_getGames_message = "Here's the log of the games played between {0} and {1}";

        public const string leaderboard_submitResult_description = "Record a game contributing to the ranking";
        public const string leaderboard_option_player1 = "Player1";
        public const string leaderboard_option_player2 = "Player2";
        public const string leaderboard_option_player3 = "Player3";
        public const string leaderboard_option_player4 = "Player4";
        public const string leaderboard_option_player1Score = "Score1";
        public const string leaderboard_option_player2Score = "Score2";
        public const string leaderboard_option_player3Score = "Score3";
        public const string leaderboard_option_player4Score = "Score4";
        public const string leaderboard_option_player1Chombo = "Chombo1";
        public const string leaderboard_option_player2Chombo = "Chombo2";
        public const string leaderboard_option_player3Chombo = "Chombo3";
        public const string leaderboard_option_player4Chombo = "Chombo4";
        public const string leaderboard_option_date = "Date";
        public const string leaderboard_option_date_description = "Time of the game (YYYY/MM/DD format)";
        public const string leaderboard_option_location = "Location";
        public const string leaderboard_option_location_description = "Where the game took place";
        public const string leaderboard_option_anyPlayer_description = "Mention (@) of the player";
        public const string leaderboard_option_anyScore_description = "Final score without uma (ie. 44300 or 44.3)";
        public const string leaderboard_option_chombo1_description = "Number of chombos done by Player1";
        public const string leaderboard_option_chombo2_description = "Number of chombos done by Player2";
        public const string leaderboard_option_chombo3_description = "Number of chombos done by Player3";
        public const string leaderboard_option_chombo4_description = "Number of chombos done by Player4";

        public const string leaderboard_submitResult_voteMessage = "2 players must :o: to confirm or :x: to cancel the game recording";
        public const string leaderboard_submitResult_messageHeader = "*place: playerName (nbGames): score => final score";
        public const string leaderboard_submitResult_NameAndId = "Name: {0}, Id: {1}";
        public const string leaderboard_submitResult_Date = "Played the : {0}";
        public const string leaderboard_submitResult_SimilarGameFound = "\n**ATTENTION**\n({0}) similar games have been found on the leaderboard, here are some details about the last one:";
        public const string leaderboard_submitResult_canceledMessage = "2 players have voted {0}, this game won't be recorded";
        public const string leaderboard_submitResult_validatedMessage = "2 players have voted {0}, this game has been recorded!";


        public const string leaderboard_submitOnlineResult_description = "Record a mahjsoul or tenhou game for the leaderboard";
        public const string leaderboard_submitOnlineResult_gameId = "gameId";
        public const string leaderboard_submitOnlineResult_gameId_description = "The tenhou or mahjsoul game id";

        public const string leaderboard_seeRanking_description = "Affiche le classement actuel de la ligue";
        public const string leaderboard_seeRanking_minGames = "nbMinGames";
        public const string leaderboard_seeRanking_minGames_description = "Minimum number of games to be in the rankings (default: 10)";
        public const string leaderboard_seeRanking_youAreHere = "You are here";

        public const string leaderboard_seeLastGames_description = "Displays the 10 last games played";
        public const string leaderboard_seeLastGames_noGames = "No game has been played between {0} and {1}";

        //Quizz commands
        public const string quizz_groupDescription = "Starts a quizz";
        public const string quizz_fullflush_description = "Starts a \"full flush\" quizz";
        public const string quizz_options_suit = "suit";
        public const string quizz_options_suit_description = "Suit to display";
        public const string quizz_options_questions = "nbQuestions";
        public const string quizz_options_questions_description = "The number of questions";
        public const string quizz_option_value_random = "random";
        public const string quizz_option_timeout = "timeout";
        public const string quizz_options_uzaku = "uzaku";
        public const string quizz_options_uzaku_description = "Limits to G.Uzaku problems [Optional]";
        public const string quizz_option_timeout_description = "Number of seconds after which a question expires (0 = sudden death)";

        public const string quizz_nanikiru_description = "Starts a nanikiru quizz";

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
        public const string mahjong_option_ukeire = "waits";
        public const string mahjong_option_ukeire_No = "No";
        public const string mahjong_option_ukeire_Yes = "Yes";
        public const string mahjong_option_ukeire_Full = "Full";
        public const string mahjong_option_ukeire_description = "Displays the number of waits of each discard (Full also displays the waits)";
        public const string mahjong_option_thread = "thread";
        public const string mahjong_option_thread_description = "Create a thread about it [Optionel]";
        public const string mahjong_nanikiru_seat = "Player {0}";
        public const string mahjong_nanikiru_round = "During {0}";
        public const string mahjong_nanikiru_turn = "Turn {0}";
        public const string mahjong_nanikiru_dora = "Dora {0}";
        public const string mahjong_nanikiru_wwyd = "What would you cut?";
        public const string mahjong_nanikiru_discussHere = "Discuss it here!";
        public const string mahjong_info_description = "Displays a hand's acceptance";

        //Ongoing Quizz
        // fullflush
        public const string quizz_fullflush_threadName = "Chinitsu agari quizz of {0} ({1} rounds)";
        public const string quizz_fullflush_startMessage = "A \"full flush\" quizz is starting.\n{0} rounds";
        public const string quizz_fullflush_questionMessageWithTime = "Question **[{0} / {1}]**. Find the tiles that complete this hand.\nYou have {2} seconds.";
        public const string quizz_fullflush_questionMessage = "Question **[{0} / {1}]**. Find the tiles that complete this hand. The first one wins!";

        //nanikiru
        public const string quizz_nanikiru_questionMessageWithTime = "Problem **[{0} / {1}]**. Find the best discard \nYou have {2} seconds.";
        public const string quizz_nanikiru_questionMessage = "Question **[{0} / {1}]**. Find the best discard. React with :eyes: to display the answer.";
        public const string quizz_nanikiru_threadName = "Nanikiru series of {0} ({1} problems)";
        public const string quizz_nanikiru_startMessage = "A series of {0} nanikiru is starting!";
        public const string quizz_nanikiru_problem_over = "(Question done)";
        public const string quizz_nanikiru_answer = "Answer:";
        public const string quizz_nanikiru_ukeire = "Ukeire:";
        public const string quizz_nanikiru_explanation = "Explanation:";
        public const string quizz_nanikiru_winners = "✅: {0}";
        public const string quizz_nanikiru_losers = "❌: {0}";

        // general
        public const string quizz_timer_disclaimer = ", {0} seconds per question.";
        public const string quizz_generatingProblem = "Displaying in 3... 2... 1...";
        public const string quizz_timeoutNoWinnerMessage = "Looks like this was a bit too hard for you!";
        public const string quizz_suddenDeathWinnerMessage = "{0} got this one first!";
        public const string quizz_answer = "The answer is {0}";
        public const string quizz_results = "Results:";
        public const string quizz_tempRanking = "Temporary ranking:";
        public const string quizz_FinalRanking = "*Final ranking:**";
        public const string quizz_isOver = "**Quizz is over!**";
        public const string quizz_error_problemAlreadyExists = "A quizz is already ongoing on this server";

        //Command attributes
        public const string commandAttributeError_title = "Command cancelled";
        public const string commandAttributeError_permissionMessage = "I don't have the required permissions in this channel";
        public const string commandAttributeError_adminMessage = "You are not this server's owner or this bot's admin";

        //Slash commands
        public const string commandError_title = "An error happened";
        public const string commandError_InvalidKanFormat = "A kan is not correct: {0}";
        public const string commandError_TooManyTiles = "Too many tiles: {0}";
        public const string commandError_CouldNotFindGameUser = "There is no leaderboard-registered user with the {0} nickname : {1}";
        public const string commandError_LeaderboardConfigRequiresScore = "The current leaderboard configuration requires players scores to record the game";
        public const string commandError_Wrong_Scores = "The sum of the scores is {0} (normally {1})";
        public const string commandError_sanmaNotAllowed = "The current leaderboard configuration doesn't accept sanma games";
        public const string commandError_badPlayerNumber = "The current leaderboard configuration does not support {0} player games";
        public const string commandError_badDistinctPlayerNumber = "There are only {0} distinct players mentioned";
        public const string commandError_unknownOnlinePlayerName = "The log has players that do not match with any current leaderboard members: {0}";
        public const string commandError_unknownLogFormat = "This log ID doesn't seem to be a mahjsoul or tenhou format";
        public const string commandError_mahjsoulUserNameChanged = "Detected old user name for <@{0}>. Automatically updated from {1} to {2}, don't thank me ;)";
        public const string commandError_gameAlreadyExists = "The game with ID: {0} already exists. A game can be recorded only once, and contribute to only one leaderboard.";
        public const string commandError_UserNicknameAlreadyExists = "A user with this name already exists and has {0} games recorded on this server.";
        public const string commandError_ValueAlreadyExists = "A user with {0} {1} exists already! Contact the admin if you want to fix that.";
        public const string commandError_CouldntExtractDateFromLog = "Couldn't extract date from this log, it won't be recorded";
        public const string commandError_PlayerUnknown = "The user {0} isn't registered";
        public const string commandError_leagueNotInitialized = "The league hasn't been created yet";
        public const string commandError_leaderboardNotInitialized = "The leaderboard hasn't been created yet";
        public const string commandError_leagueAlreadyInitialized = "An league is still ongoing on this server";
        public const string commandError_noOngoingLeague = "There is no ongoing league on this server";
        public const string commandError_leaderboardAlreadyInitialized = "The leaderboard has already been created";
        public const string commandError_teamNameAlreadyExists = "A team with this name already exists in this league";
        public const string commandError_playerAlreadyPresentInATeam = "This user is already in a team for this league (maybe this one?)";
        public const string commandError_teamDoesNotExist = "The team {0} doesn't exist in this league";
        public const string commandError_PlayerNotOnRiichiCity = "The user {0} doesn't have a Riichi City ID";
        public const string commandError_PlayerNotInAteam = "The player {0} isn't in any team of the current league";
        public const string commandError_RiichiCityConnectionFailed = "The connection to Riichi City failed";
        public const string commandError_playerAlreadySubbed = "Player is already subbed out for this game (ask admin if you want to change that)";


        //Leaderboard
        public const string kandoraLeaderboard_roleName = "Kandora Leaderboard";
    }
}
