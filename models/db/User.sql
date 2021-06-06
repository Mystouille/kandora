CREATE TABLE [dbo].[User]
(
	[Id]  NVARCHAR(20) NOT NULL UNIQUE, 
    [displayName] NVARCHAR(200) NOT NULL, 
    [mahjsoulId] NVARCHAR(30) NOT NULL,
    [tenhouId] NVARCHAR(30) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
);
