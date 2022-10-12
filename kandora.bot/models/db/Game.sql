CREATE TABLE [dbo].[Game] (
    [Id]  NVARCHAR (100) PRIMARY KEY,
    [platform]  NVARCHAR (100) NOT NULL,
    [user1Id]     NVARCHAR (20) NOT NULL,
    [user1Score]  INT NOT NULL,
    [user2Id]     NVARCHAR (20) NOT NULL,
    [user2Score]  INT NOT NULL,
    [user3Id]     NVARCHAR (20) NOT NULL,
    [user3Score]  INT NOT NULL,
    [user4Id]     NVARCHAR (20) NOT NULL,
    [user4Score]  INT NOT NULL,
    [isSanma]
    [timestamp]   DATETIME   NOT NULL DEFAULT SYSDATETIME(),
    [serverId]    NVARCHAR (20) NOT NULL,
    [fullLog]  NVARCHAR(MAX) NULL, 
    FOREIGN KEY ([user1Id]) REFERENCES [dbo].[User] ([Id]),
    FOREIGN KEY ([user2Id]) REFERENCES [dbo].[User] ([Id]),
    FOREIGN KEY ([user3Id]) REFERENCES [dbo].[User] ([Id]),
    FOREIGN KEY ([user4Id]) REFERENCES [dbo].[User] ([Id]),
    FOREIGN KEY ([serverId]) REFERENCES [dbo].[Server] ([Id])
);