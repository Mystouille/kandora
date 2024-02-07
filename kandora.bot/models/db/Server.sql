CREATE TABLE [dbo].[Server] (
    [Id] NVARCHAR(20) NOT NULL UNIQUE,
    [displayName] NVARCHAR(200) NOT NULL,
    [leaderboardRoleId] NVARCHAR(20) NOT NULL,
    [leaderboardName] NVARCHAR(30) NOT NULL,
    [targetChannelId] NVARCHAR(20) UNIQUE NULL,
    leaderboardConfigId INT NOT NULL,
    FOREIGN KEY (leaderboardConfigId) REFERENCES [dbo].[LeaderboardConfig] ([Id])
);