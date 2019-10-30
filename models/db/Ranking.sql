﻿CREATE TABLE [dbo].[Ranking] (
    [Id]        INT        NOT NULL,
    [userId]    INT        NOT NULL,
    [elo]       NCHAR (10) NULL,
    [timestamp] ROWVERSION NOT NULL,
    [gameId]    INT        NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([Id])
);

