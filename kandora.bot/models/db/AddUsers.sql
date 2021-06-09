

CREATE TABLE [dbo].[User]
(
	[Id]  NVARCHAR(20) NOT NULL UNIQUE,
    [mahjsoulFriendId] NVARCHAR(30) NULL, 
    [mahjsoulUserId] NVARCHAR(30) NULL,
    [mahjsoulName] NVARCHAR(30) NULL,
    [tenhouName] NVARCHAR(30) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
);
