CREATE TABLE [dbo].[ServerUser] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [userId]  NVARCHAR(20) NOT NULL, 
    [serverId] NVARCHAR(20) NOT NULL,
    [isAdmin] BIT NOT NULL,
    [isOwner] BIT NOT NULL,
    FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([Id]),
    FOREIGN KEY ([serverId]) REFERENCES [dbo].[Server] ([Id]),
);