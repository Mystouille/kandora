CREATE TABLE [dbo].[Ranking] (
    [Id]        INT  NOT NULL IDENTITY(1,1) PRIMARY KEY,
    [userId]    NCHAR(20) NOT NULL,
    [oldElo]    FLOAT NULL,
    [newElo]    FLOAT NOT NULL,
	[position]	INT		   NULL,
    [timestamp] DATETIME   NOT NULL DEFAULT SYSDATETIME(),
    [gameId]    INT        NULL,
    FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([Id]),
    FOREIGN KEY ([gameId]) REFERENCES [dbo].[Game] ([Id])
);

