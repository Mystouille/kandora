CREATE TABLE [dbo].[Validation]
(
	[Id] INT NOT NULL IDENTITY(1,1), 
    [userId] INT NOT NULL, 
	FOREIGN KEY (userId) REFERENCES [User] ([Id]),
    [gameId] INT NOT NULL, 
	FOREIGN KEY (gameId) REFERENCES [Game] ([Id]),
    [validated] BIT NOT NULL DEFAULT 0
    PRIMARY KEY CLUSTERED ([Id] ASC),
)
