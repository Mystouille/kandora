CREATE TABLE [dbo].[User]
(
	[Id]  NCHAR(20) NOT NULL UNIQUE, 
    [displayName] NCHAR(200) NOT NULL, 
    [mahjsoulId] NCHAR(30) NOT NULL,
    [tenhouId] NCHAR(30) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
);
