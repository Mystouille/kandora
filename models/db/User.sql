CREATE TABLE [dbo].[User]
(
	[Id] INT NOT NULL IDENTITY(1,1), 
    [displayName] NCHAR(30) NOT NULL, 
    [discordId] NUMERIC(20,0) NOT NULL UNIQUE, 
    [mahjsoulId] INT NOT NULL
    PRIMARY KEY CLUSTERED ([Id] ASC),
)
