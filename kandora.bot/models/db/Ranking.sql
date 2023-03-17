CREATE TABLE [dbo].[Ranking] (
    [Id]        INT  NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [userId]    NVARCHAR(20) NOT NULL,
    [oldElo]    FLOAT NULL,
    [newElo]    FLOAT NOT NULL,
	[position]	INT		   NULL,
	[finalscore]INT		   NULL,
    [timestamp] DATETIME   NOT NULL DEFAULT SYSDATETIME(),
    [gameId]    NVARCHAR(100)   NULL,
    [serverId]  NVARCHAR(20) NOT NULL,
    FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([Id]),
    FOREIGN KEY ([gameId]) REFERENCES [dbo].[Game] ([Id]),
    FOREIGN KEY ([serverId]) REFERENCES [dbo].[Server] ([Id])
);

