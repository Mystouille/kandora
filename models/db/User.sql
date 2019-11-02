CREATE TABLE [dbo].[User]
(
	[Id]  NUMERIC(20,0) NOT NULL UNIQUE, 
    [displayName] NCHAR(30) NOT NULL, 
    [mahjsoulId] INT NOT NULL,
    [isAdmin] BIT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
)
