CREATE TABLE [dbo].[Game]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [mahjsoulId] NCHAR(50) NULL, 
    [user1Id] INT NOT NULL,
	FOREIGN KEY (user1Id) REFERENCES [User] ([Id]),
    [user1Score] INT NOT NULL, 
    [user2Id] INT NOT NULL, 
	FOREIGN KEY (user2Id) REFERENCES [User] ([Id]),
    [user2Score] INT NOT NULL, 
    [user3Id] INT NOT NULL, 
	FOREIGN KEY (user3Id) REFERENCES [User] ([Id]),
    [user3Score] INT NOT NULL, 
    [user4Id] INT NOT NULL, 
	FOREIGN KEY (user4Id) REFERENCES [User] ([Id]),
    [user4Score] INT NOT NULL, 
    [timestamp] TIMESTAMP 
)
