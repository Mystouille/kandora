CREATE TABLE [dbo].[Platform] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [platformName] NVARCHAR(200) NOT NULL,
);
INSERT INTO [dbo].[Platform] (platformName)
VALUES ('Mahjong Soul'), ('Tenhou'), ('Autotable'), ('IRL'), ('Other');
