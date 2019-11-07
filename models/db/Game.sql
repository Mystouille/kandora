﻿CREATE TABLE [dbo].[Game] (
    [Id]          INT          NOT NULL,
    [mahjsoulId]  NCHAR (50)   NULL,
    [user1Id]     NUMERIC (20) NOT NULL,
    [user1Score]  INT          ,
    [user1Signed] BIT          DEFAULT ((0)) NOT NULL,
    [user2Id]     NUMERIC (20) NOT NULL,
    [user2Score]  INT          ,
    [user2Signed] BIT          DEFAULT ((0)) NOT NULL,
    [user3Id]     NUMERIC (20) NOT NULL,
    [user3Score]  INT          ,
    [user3Signed] BIT          DEFAULT ((0)) NOT NULL,
    [user4Id]     NUMERIC (20) NOT NULL,
    [user4Score]  INT          ,
    [user4Signed] BIT          DEFAULT ((0)) NOT NULL,
    [timestamp]   DATETIME   NOT NULL DEFAULT SYSDATETIME(),
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([user1Id]) REFERENCES [dbo].[User] ([Id]),
    FOREIGN KEY ([user2Id]) REFERENCES [dbo].[User] ([Id]),
    FOREIGN KEY ([user3Id]) REFERENCES [dbo].[User] ([Id]),
    FOREIGN KEY ([user4Id]) REFERENCES [dbo].[User] ([Id])
)