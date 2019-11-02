﻿CREATE TABLE [dbo].[Ranking] (
    [Id]        INT        NOT NULL IDENTITY(1,1),
    [userId]    NUMERIC(20,0) NOT NULL,
    [elo]       NCHAR (10) NULL,
    [timestamp] ROWVERSION NOT NULL,
    [gameId]    INT        NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([userId]) REFERENCES [dbo].[User] ([Id]),
    FOREIGN KEY ([gameId]) REFERENCES [dbo].[Game] ([Id])
);

