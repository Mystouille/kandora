using kandora.bot.models;

namespace kandora.bot.resources
{
    class Resources
    {
        public const string cultureInfoStr = "fr-FR";

        //Admin commands
        public const string admin_groupDescription = "Commandes d'administation";
        public const string admin_startLeaderboard_description = "Commence un leaderboard de mahjong";
        public const string admin_startLeaderboard_leaderboardStarted = "Un leaderboard vient de commencer sur {0}!";
        public const string admin_startLeaderboard_leaderboardAlreadyExists = "Un leaderboard existe déjà sur {0}!";
        public const string admin_startLeague_description = "Commence une ligue en equipe de mahjong";
        public const string admin_startLeague_tournamentId = "tournamentId";
        public const string admin_startLeague_tournamentId_description = "L'ID d'un tournoi riichi city";
        public const string admin_startLeague_displayName = "displayName";
        public const string admin_startLeague_displayName_description = "le nom de la ligue";
        public const string admin_startLeague_cutoff = "finalsCutoff";
        public const string admin_startLeague_cutoff_description = "La date début de la phase finale (yyyy/MM/dd)";
        public const string admin_startLeague_leagueStarted = "Une ligue vient de commencer sur {0}!";
        public const string admin_startLeague_leagueAlreadyExists = "Une ligue  existe déjà sur {0}!";
        public const string admin_endLeague_description = "Termine la ligue en cours";
        public const string admin_endLeague_leagueEnded = "{0} est terminée";
        public const string admin_editLeague_description = "Change les paramètre de la ligue";
        public const string admin_editLeague_leagueChanged = "La ligue actuelle a été modifiée";
        public const string admin_addSub_userOut = "absent";
        public const string admin_addSub_userOut_description = "Le joueur qui était absent (@ mention ou nom d'invité)";
        public const string admin_addSub_userIn = "remplacant";
        public const string admin_addSub_userIn_description = "Le joueur qui l'a remplacé (@ mention ou nom d'invité)";
        public const string admin_addSub_gameId = "gameId";
        public const string admin_addSub_gameId_description = "L'id de partie Riici City (log id)";
        public const string admin_addSub_playerHasBeenSubbed = "Le joueur {0} a été remplacé par {1} pour la partie {2}";
        public const string admin_createTeam_description = "Crée une équipe pour la ligue actuelle";
        public const string admin_createTeam_teamName = "teamName";
        public const string admin_createTeam_teamName_description = "Le nom de l'équipe (sans espace ni caractères spéciaux)";
        public const string admin_createTeam_fancyName = "fancyName";
        public const string admin_createTeam_fancyName_description = "Le nom d'affichage de l'équipe (faites vous plaisir sur les emojis)";
        public const string admin_createTeam_teamCreated = "L'équipe \"{0}\" a été créée dans {1}!";
        public const string admin_addTeamPlayer_description = "Ajoute un joueur à une équipe";
        public const string admin_addTeamPlayer_user = "userName";
        public const string admin_addTeamPlayer_user_description = "Le nom ou la mention (@) du joueur";
        public const string admin_addTeamPlayer_team = "teamName";
        public const string admin_addTeamPlayer_team_description = "Le nom de l'équipe";
        public const string admin_addTeamPlayer_userHasBeenAdded = "Le joueur {0} a bien été ajouté à l'équipe {1}!";
        public const string admin_flushServer_description = "Supprime une ligue de mahjong ainsi que l'historique des parties";
        public const string admin_flushServer_unauthorized = "Cette commande supprime tout l'historique, on va éviter pour le moment.";
        public const string admin_flushServer_leaderboardEnded = "La ligue de mahjong a été supprimée!";
        public const string admin_testLeaderboard_description = "Supprime la ligue actuelle et en crée une composée de 4 joueurs";
        public const string admin_testLeaderboard_testLeaderboardStarted = "Une ligue de test a été créée avec ces joueurs:\n{0}";
        public const string admin_showConfig_description = "Affiche les paramètres actuels de la ligue ou du leaderboard";
        public const string admin_seeConfig_type_league = "Ligue en équipe";
        public const string admin_seeConfig_type_leaderboard = "Leaderboard";
        public const string admin_seeConfig_type = "type";
        public const string admin_seeConfig_type_description = "La config voulue";
        public const string admin_setLeaderboardConfig_description = "Configure la ligue";
        public const string admin_setLeaderboardConfig_allowSanma = "allowSanma";
        public const string admin_setLeaderboardConfig_allowSanma_description = "Accepte les parties à 3 joueurs";
        public const string admin_setLeaderboardConfig_countPoints = "countPoints";
        public const string admin_setLeaderboardConfig_eloSystem_None = "Aucun";
        public const string admin_setLeaderboardConfig_eloSystem_Average = "Moyenne";
        public const string admin_setLeaderboardConfig_eloSystem_Simple = "Simple";
        public const string admin_setLeaderboardConfig_eloSystem_Full = "Complet";
        public const string admin_setLeaderboardConfig_countPoints_description = "Prend en compte les points";
        public const string admin_setLeaderboardConfig_eloSystem = "eloSystem";
        public const string admin_setLeaderboardConfig_eloSystem_description = "Selectione le systèle de ELO";
        public const string admin_setLeaderboardConfig_startTime = "dateDebut";
        public const string admin_setLeaderboardConfig_startTime_description = "La date de debut de la ligue (format: AAAA/MM/JJ)";
        public const string admin_setLeaderboardConfig_endTime = "dateFin";
        public const string admin_setLeaderboardConfig_endTime_description = "La date de fin de la ligue (format: AAAA/MM/JJ)";
        public const string admin_setLeaderboardConfig_startingPoints = "startingPoints";
        public const string admin_setLeaderboardConfig_startingPoints_description = "Points de chaque joueur en début de partie (en milliers)";
        public const string admin_setLeaderboardConfig_uma3p1 = "uma3p1";
        public const string admin_setLeaderboardConfig_uma3p1_description = "Uma du premier joueur en sanma";
        public const string admin_setLeaderboardConfig_uma3p2 = "uma3p2";
        public const string admin_setLeaderboardConfig_uma3p2_description = "Uma du deuxième joueur en sanma";
        public const string admin_setLeaderboardConfig_uma3p3 = "uma3p3";
        public const string admin_setLeaderboardConfig_uma3p3_description = "Uma du dernier joueur en sanma";
        public const string admin_setLeaderboardConfig_uma4p1 = "uma4p1";
        public const string admin_setLeaderboardConfig_uma4p1_description = "Uma du premier joueur";
        public const string admin_setLeaderboardConfig_uma4p2 = "uma4p2";
        public const string admin_setLeaderboardConfig_uma4p2_description = "Uma du deuxième joueur";
        public const string admin_setLeaderboardConfig_uma4p3 = "uma4p3";
        public const string admin_setLeaderboardConfig_uma4p3_description = "Uma du troisième joueur";
        public const string admin_setLeaderboardConfig_uma4p4 = "uma4p4";
        public const string admin_setLeaderboardConfig_uma4p4_description = "Uma du dernier joueur";
        public const string admin_setLeaderboardConfig_oka = "oka";
        public const string admin_setLeaderboardConfig_oka_description = "Oka";
        public const string admin_setLeaderboardConfig_penaltyLast = "penaltyLast";
        public const string admin_setLeaderboardConfig_penaltyLast_description = "Pénalité de dernière place";
        public const string admin_setLeaderboardConfig_penaltyChombo = "penaltyChombo";
        public const string admin_setLeaderboardConfig_penaltyChombo_description = "Pénalité par chombo";
        public const string admin_setLeaderboardConfig_initialElo = "initialElo";
        public const string admin_setLeaderboardConfig_initialElo_description = "ELO initial";
        public const string admin_setLeaderboardConfig_minElo = "minElo";
        public const string admin_setLeaderboardConfig_minElo_description = "ELO minimum";
        public const string admin_setLeaderboardConfig_eloChangeDampening = "eloChangeDampening";
        public const string admin_setLeaderboardConfig_eloChangeDampening_description = "amortissement de la variation de ELO";
        public const string admin_setLeaderboardConfig_eloChangeStartRatio = "eloChangeStartRatio";
        public const string admin_setLeaderboardConfig_eloChangeStartRatio_description = "amortissement de départ";
        public const string admin_setLeaderboardConfig_eloChangeEndRatio = "eloChangeEndRatio";
        public const string admin_setLeaderboardConfig_eloChangeEndRatio_description = "amortissement final";
        public const string admin_setLeaderboardConfig_trialPeriodDuration = "trialPeriodDuration";
        public const string admin_setLeaderboardConfig_trialPeriodDuration_description = "Nombre de parties avec un faible amortissement";
        public const string admin_setLeaderboardConfig_backfillInProgress = "La configuration de la ligue a changé.\n{0}\n\n :bangbang: Kandora recalcule le classement à partir du début... ";
        public const string admin_setLeaderboardConfig_backfillFinished = "La configuration de la ligue a changé.\n{0}\n\n :white_check_mark: Kandora a fini de recalculer le classement! ";

        public const string admin_addPlayer_description = "Inscrit un utilisateur au leaderboard du serveur";
        public const string admin_addPlayer_nickname = "nom";
        public const string admin_addPlayer_nickname_description = "La mention du joueur (avec @) ou son nom d'invité s'il n'est pas sur le serveur";
        public const string admin_addPlayer_Success = "Le joueur {0} a bien été ajouté au leaderboard";
        public const string admin_setUser_description = "Modifie les infos d'un utilisateur (à utiliser avec modération)";
        public const string admin_setUser_Success = "Le joueur {0} a bien été modifié";


        public const string admin_migratePlayer_description = "Change le pseudo d'un utilisateur (et migre son historique de parties)";
        public const string admin_migratePlayer_sourceName = "pseudoActuel";
        public const string admin_migratePlayer_sourceName_description = "Le pseudo du joueur invité ou la mention du joueur (avec @)";
        public const string admin_migratePlayer_targetName = "pseudoVoulu";
        public const string admin_migratePlayer_targetName_description = "Mention du joueur voulu ou pseudo (pour crée un joueur invité)";
        public const string admin_migratePlayer_success = "Le joueur {0} a bien été renommé en {1}.";

        //League commands
        public const string league_groupDescription = "Commandes de ligue";
        public const string admin_startGame_description = "Commence une partie de ligue";
        public const string admin_startGame_user1 = "user1";
        public const string admin_startGame_user1_description = "La mention (@) ou nom du joueur 1";
        public const string admin_startGame_user2 = "user2";
        public const string admin_startGame_user2_description = "La mention (@) ou nom du joueur 2";
        public const string admin_startGame_user3 = "user3";
        public const string admin_startGame_user3_description = "La mention (@) ou nom du joueur 3";
        public const string admin_startGame_user4 = "user4";
        public const string admin_startGame_user4_description = "La mention (@) ou nom du joueur 4";
        public const string admin_startGame_gameStarted = "La partie à commencé!";
        public const string admin_seeLeagueRanking_description = "Affiche le classement de la ligue en cours";
        public const string admin_seeLeagueRanking_displayInChat = "displayInChat";
        public const string admin_seeLeagueRanking_displayInChat_description = "Affiche le score publiquement";


        //Leaderboard commands
        public const string leaderboard_groupDescription = "Commandes du leaderboard";
        public const string leaderboard_register_description = "S'inscrire à la ligue ou modifier ses infos";
        public const string leaderboard_register_mahjsoulName = "mahjsoulname";
        public const string leaderboard_register_mahjsoulName_description = "Ton pseudo Mahjong Soul";
        public const string leaderboard_register_mahjsoulFriendId = "mahjsoulid";
        public const string leaderboard_register_mahjsoulFriendId_description = "Ton ID Mahjong Soul";
        public const string leaderboard_register_tenhouName = "tenhouname";
        public const string leaderboard_register_tenhouName_description = "Ton pseudo Tenhou";
        public const string leaderboard_register_riichiCityId = "riichicityid";
        public const string leaderboard_register_riichiCityId_description = "L'ID utilisateur Riichi City";
        public const string leaderboard_register_riichiCityName = "riichicityname";
        public const string leaderboard_register_riichiCityName_description = "Le pseudo Riichi City";
        public const string leaderboard_register_response_newUser = "Tu fais maintenant partie de la ligue de ce serveur";
        public const string leaderboard_register_response_newRanking = "Tu fais maintenant partie de la ligue de ce serveur. Attention, tes pseudos Tenhou et Mahjong Soul sont communs avec les ligues d'autres serveurs";
        public const string leaderboard_register_response_userAlreadyRegistered = "Les nouvelles infos ont été prise en compte";

        public const string leaderboard_logInfo_description = "Affiche les infos d'une partie à partir de son ID";
        public const string leaderboard_option_gameId = "id";
        public const string leaderboard_option_gameId_description = "L'ID de la partie (tenhou ou mahjsoul)";
        public const string leaderboard_title = "Titre"; 
        public const string leaderboard_date = "Date";
        public const string leaderboard_results = "Résultats"; 
        public const string leaderboard_bestHand = "Meilleure main:  {0}({1}) (manche {2}) avec {3} pour un total de {4} ";

        public const string leaderboard_getGames_description = "Télécharge un fichier csv des parties jouées pendant la ligue";
        public const string leaderboard_getGames_fileHeader = "logId,nomPartie,lieu,date,joueur1,score1,chombo1,joueur2,score2,chombo2,joueur3,score3,chombo3,joueur4,score4,chombo4";
        public const string leaderboard_getGames_message = "Voila le log des parties jouées entre le {0} et le {1}";

        public const string leaderboard_submitResult_description = "Enregistre une partie comptant dans le classement";
        public const string leaderboard_option_player1 = "Joueur1";
        public const string leaderboard_option_player2 = "Joueur2";
        public const string leaderboard_option_player3 = "Joueur3";
        public const string leaderboard_option_player4 = "Joueur4";
        public const string leaderboard_option_player1Score = "Score1";
        public const string leaderboard_option_player2Score = "Score2";
        public const string leaderboard_option_player3Score = "Score3";
        public const string leaderboard_option_player4Score = "Score4";
        public const string leaderboard_option_player1Chombo = "Chombo1";
        public const string leaderboard_option_player2Chombo = "Chombo2";
        public const string leaderboard_option_player3Chombo = "Chombo3";
        public const string leaderboard_option_player4Chombo = "Chombo4";
        public const string leaderboard_option_date = "Date";
        public const string leaderboard_option_date_description = "Date de la partie (au format YYYY/MM/DD)";
        public const string leaderboard_option_location = "Lieu";
        public const string leaderboard_option_location_description = "Endroit ou s'est passée la partie";
        public const string leaderboard_option_anyPlayer_description = "Mention (@) du joueur";
        public const string leaderboard_option_anyScore_description = "Score final sans uma (ex: 44300 ou 44.3)";
        public const string leaderboard_option_chombo1_description = "Nombre de chombos pour le Joueur1";
        public const string leaderboard_option_chombo2_description = "Nombre de chombos pour le Joueur2";
        public const string leaderboard_option_chombo3_description = "Nombre de chombos pour le Joueur3";
        public const string leaderboard_option_chombo4_description = "Nombre de chombos pour le Joueur4";

        public const string leaderboard_submitResult_voteMessage = "2 joueurs doivent :o: pour valider ou :x: pour annuler l'enregistrement de la partie.";
        public const string leaderboard_submitResult_messageHeader = "*place: nomJoueur (nbparties): score => score final*";
        public const string leaderboard_submitResult_NameAndId = "Nom: {0}, Id: {1}";
        public const string leaderboard_submitResult_Date = "Partie du : {0}";
        public const string leaderboard_submitResult_SimilarGameFound = "\n**ATTENTION**\n({0}) parties similaires ont été trouvées sur la leaderboard, voici les details de la dernière en date:"; 
        public const string leaderboard_submitResult_canceledMessage = "2 joueurs ont voté {0}, cette partie ne sera pas enregistrée";
        public const string leaderboard_submitResult_validatedMessage = "2 joueurs ont voté {0}, cette partie a été enregistrée!";

        public const string leaderboard_submitOnlineResult_description = "Enregistre une partie mahjsoul ou tenhou comptant dans le classement";
        public const string leaderboard_submitOnlineResult_gameId = "gameId";
        public const string leaderboard_submitOnlineResult_gameId_description = "L'ID de la partie Mahjsoul ou Tenhou";


        public const string leaderboard_seeRanking_description = "Affiche le classement actuel de la ligue";
        public const string leaderboard_seeRanking_minGames = "nbPartiesMin";
        public const string leaderboard_seeRanking_minGames_description = "Nombre de parties minimum pour être dans le classement (défaut: 10)";
        public const string leaderboard_seeRanking_youAreHere = "Tu es là";

        public const string leaderboard_seeLastGames_description = "Affiche les 10 dernières parties jouées";
        public const string leaderboard_seeLastGames_noGames = "Aucune partie n'a été jouée entre le {0} et le {1}";
        


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
        public const string mahjong_info_description = "Affiche les améliorations d'une main";


        //Ongoing Quizz
        // chinitsu
        public const string quizz_fullflush_threadName = "Quizz chinitsu agari du {0} ({1} manches)";
        public const string quizz_fullflush_startMessage = "Un quizz de main pure commence.\n{0} manches";
        public const string quizz_fullflush_questionMessageWithTime = "Question **[{0} / {1}]**. Trouvez les tuiles qui completent la main.\nVous avez {2} secondes.";
        public const string quizz_fullflush_questionMessage = "Question **[{0} / {1}]**. Trouvez les tuiles qui completent la main. Seul le premier gagne!";

        // general
        public const string quizz_timer_disclaimer = ", {0} secondes par question.";
        public const string quizz_generatingProblem = "Affichage dans 3... 2... 1...";
        public const string quizz_timeoutNoWinnerMessage = "Aucun gagnant, c'était peut-être un peu trop dur?";
        public const string quizz_suddenDeathWinnerMessage = "{0} a trouvé en premier!";
        public const string quizz_answer = "La réponse est {0}";
        public const string quizz_results = "Résultats:";
        public const string quizz_tempRanking = "Classement temporaire:";
        public const string quizz_FinalRanking = "**Classement final:**";
        public const string quizz_isOver = "**Le quizz est terminé!**";
        public const string quizz_error_problemAlreadyExists = "Un quizz est déjà en cours sur ce serveur";

        //Command attributes
        public const string commandAttributeError_title = "Commande annulée";
        public const string commandAttributeError_permissionMessage = "Je n'ai pas les permissions nécessaires dans ce salon";
        public const string commandAttributeError_adminMessage = "Tu n'es pas owner du server ou admin du bot";

        //Slash commands
        public const string commandError_title = "Une erreur s'est produite";
        public const string commandError_InvalidKanFormat = "Un kan est mal déclaré: {0}";
        public const string commandError_TooManyTiles = "Trop de tuiles: {0}";
        public const string commandError_CouldNotFindGameUser = "Il n'y a pas de joueur inscrit avec le nom {0} : {1}";
        public const string commandError_LeaderboardConfigRequiresScore = "La configuration actuelle de la ligue nécessite les scores des joueurs pour enregistrer une partie.";
        public const string commandError_Wrong_Scores = "La somme des points des joueurs est {0} (normalement {1})";
        public const string commandError_sanmaNotAllowed = "La configuration actuelle de la ligue n'accepte pas le sanma";
        public const string commandError_badPlayerNumber = "La configuration actuelle de la ligue n'accepte pas les parties a {0} joueurs";
        public const string commandError_badDistinctPlayerNumber = "Il n'y a que {0} joueurs distincts mentionés";
        public const string commandError_unknownOnlinePlayerName = "Ce log contient des joueurs qui ne sont pas présent dans la ligue: {0}";
        public const string commandError_unknownLogFormat = "Cet ID de log n'a pas l'air d'être au format tenhou ou mahjsoul";
        public const string commandError_mahjsoulUserNameChanged = "Le pseudo mahjsoul de <@{0}> semble avoir changé. Il vient d'être automatiquement mis à jour de {1} vers {2}. De rien ;)";
        public const string commandError_gameAlreadyExists = "La partie ayant pour id: {0} existe déjà. Une partie ne peut être enregistrée qu'une fois, et que dans une seule ligue à la fois.";
        public const string commandError_UserNicknameAlreadyExists = "Un utilisateur avec ce nom existe déja et a {0} partie(s) sur ce serveur.";
        public const string commandError_ValueAlreadyExists = "Un joueur ayant comme {0}: {1} existe déjà! Contacte un admin si nécessaire.";
        public const string commandError_CouldntExtractDateFromLog = "La date de ce log n'a pas pu être extraite. la partie ne sera pas enregistrée";
        public const string commandError_PlayerUnknown = "Le joueur {0} n'est pas encore inscrit";
        public const string commandError_leaderboardNotInitialized = "Le leaderboard n'a pas été encore crée";
        public const string commandError_leagueNotInitialized = "La ligue n'a pas été encore crée";
        public const string commandError_leaderboardAlreadyInitialized = "Le leaderboard a déjà été créé";
        public const string commandError_leagueAlreadyInitialized = "Une ligue est déjà en cours sur ce serveur";
        public const string commandError_noOngoingLeague = "Aucune ligue n'est en cours sur ce serveur";
        public const string commandError_teamNameAlreadyExists = "Une équipe avec ce nom existe déjà dans cette ligue";
        public const string commandError_playerAlreadyPresentInATeam = "Ce joueur est déjà présent dans une équipe pour cette ligue";
        public const string commandError_teamDoesNotExist = "L'équipe {0} n'existe pas dans cette ligue";
        public const string commandError_PlayerNotOnRiichiCity = "Le joueur {0} n'a pas d'ID Riichi City";
        public const string commandError_PlayerNotInAteam = "Le joueur {0} n'est dans aucun equipe de la ligue actuelle";
        public const string commandError_RiichiCityConnectionFailed = "La connection avec Riichi City a échoué";
        public const string commandError_playerAlreadySubbed = "Le joueur a déjà été remplacé dans cette partie";







        //Leaderboard
        public const string kandoraLeaderboard_roleName = "Ligue Kandora";
    }
}
