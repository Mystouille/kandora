CREATE TABLE [dbo].[Server] (
    [Id] NCHAR(20) NOT NULL UNIQUE,
    [displayName] NCHAR(200) NOT NULL,
    [targetChannelId] NCHAR(20) UNIQUE,
);