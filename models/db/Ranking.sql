CREATE TABLE [dbo].[Ranking] (
    [Id]        INT        NOT NULL IDENTITY(1,1),
    [userId]    NUMERIC(20,0) NOT NULL,
    [oldElo]    FLOAT NULL,
    [newElo]    FLOAT NOT NULL,
	[position]	INT		   NULL,
    [timestamp] DATETIME   NOT NULL DEFAULT SYSDATETIME(),
    [gameId]    INT        NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([Id]),
    FOREIGN KEY ([gameId]) REFERENCES [dbo].[Game] ([Id])
);

