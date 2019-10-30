CREATE TABLE [dbo].[User]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [displayName] NCHAR(30) NOT NULL, 
    [uniqueName] NCHAR(30) NOT NULL, 
    [mahjsoulId] NCHAR(30) NOT NULL
)
