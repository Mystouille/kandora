DROP TABLE Ranking;
DROP TABLE Game;
DROP TABLE ServerUser;
DROP TABLE Server;
DROP TABLE DiscordUser;
DROP TABLE LeagueConfig;
CREATE TABLE DiscordUser
(
	Id  VARCHAR(20) PRIMARY KEY NOT NULL UNIQUE,
    mahjsoulFriendId VARCHAR(30) NULL, 
    mahjsoulUserId VARCHAR(30) NULL,
    mahjsoulName VARCHAR(30) NULL,
    tenhouName VARCHAR(30) NULL,
    leaguePassword VARCHAR NULL
);
CREATE TABLE LeagueConfig (
    Id SERIAL PRIMARY KEY,
    countPoints BOOL NOT NULL,
    allowSanma BOOL DEFAULT false NOT NULL,
    startingPoints FLOAT,
    uma3p1 FLOAT,
    uma3p2 FLOAT,
    uma3p3 FLOAT,
    uma4p1 FLOAT,
    uma4p2 FLOAT,
    uma4p3 FLOAT,
    uma4p4 FLOAT,
    oka FLOAT,
    penaltyLast FLOAT,
    useEloSystem BOOL NOT NULL,
    initialElo FLOAT,
    minElo FLOAT,
    baseEloChangeDampening FLOAT,
    eloChangeStartRatio FLOAT,
    eloChangeEndRatio FLOAT,
    trialPeriodDuration INT
);
CREATE TABLE Server (
    Id VARCHAR(20) PRIMARY KEY NOT NULL UNIQUE,
    displayName VARCHAR(200) NOT NULL,
    leagueRoleId VARCHAR(20) NOT NULL,
    leagueName VARCHAR(30) NOT NULL,
    leagueId INT NOT NULL,
    FOREIGN KEY (leagueId) REFERENCES LeagueConfig (Id)
);
CREATE TABLE ServerUser (
    Id SERIAL PRIMARY KEY,
    userId  VARCHAR(20) NOT NULL, 
    serverId VARCHAR(20) NOT NULL,
    isAdmin BOOL NOT NULL,
    FOREIGN KEY (userId) REFERENCES DiscordUser (Id),
    FOREIGN KEY (serverId) REFERENCES Server (Id)
);
CREATE TABLE Game (
    Id  VARCHAR (100) PRIMARY KEY,
    platform  VARCHAR (100) NOT NULL,
    user1Id     VARCHAR (20) NOT NULL,
    user1Score  INT          ,
    user2Id     VARCHAR (20) NOT NULL,
    user2Score  INT          ,
    user3Id     VARCHAR (20) NOT NULL,
    user3Score  INT          ,
    user4Id     VARCHAR (20) NOT NULL,
    user4Score  INT          ,
    timestamp timestamp default current_timestamp,
    serverId    VARCHAR (20) NOT NULL,
    fullLog  VARCHAR NULL, 
    FOREIGN KEY (user1Id) REFERENCES DiscordUser (Id),
    FOREIGN KEY (user2Id) REFERENCES DiscordUser (Id),
    FOREIGN KEY (user3Id) REFERENCES DiscordUser (Id),
    FOREIGN KEY (user4Id) REFERENCES DiscordUser (Id),
    FOREIGN KEY (serverId) REFERENCES Server (Id)
);
CREATE TABLE Ranking (
    Id SERIAL PRIMARY KEY,
    userId VARCHAR(20) NOT NULL,
    oldElo    FLOAT NULL,
    newElo    FLOAT NOT NULL,
	position INT NULL,
    timestamp timestamp default current_timestamp,
    gameId    VARCHAR(100)   NULL,
    serverId  VARCHAR(20) NOT NULL,
    FOREIGN KEY (userId) REFERENCES DiscordUser (Id),
    FOREIGN KEY (gameId) REFERENCES Game (Id),
    FOREIGN KEY (serverId) REFERENCES Server (Id)
);