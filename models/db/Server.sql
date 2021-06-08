CREATE TABLE [dbo].[Server] (
    [Id] NVARCHAR(20) NOT NULL UNIQUE,
    [displayName] NVARCHAR(200) NOT NULL,
    [leagueRoleId] NVARCHAR(20) NOT NULL,
    [leagueName] NVARCHAR(30) NOT NULL,
    [targetChannelId] NVARCHAR(20) UNIQUE NULL,
    [leagueId] INT NOT NULL,
    FOREIGN KEY ([leagueId]) REFERENCES [dbo].[LeagueConfig] ([Id])
);