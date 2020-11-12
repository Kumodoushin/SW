CREATE TABLE [swapi].[CharacterEpisode]
(
	[Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() NOT NULL,
	[CharacterId] UNIQUEIDENTIFIER NOT NULL,
	[Episode] INT NOT NULL,
);
GO

ALTER TABLE [swapi].[CharacterEpisode]
	ADD CONSTRAINT PK_CharacterEpisode PRIMARY KEY CLUSTERED([Id]);
GO

ALTER TABLE [swapi].[CharacterEpisode]
	ADD CONSTRAINT FK_CharacterEpisode_CharacterId FOREIGN KEY ([CharacterId]) REFERENCES [swapi].[Character]([Id]);
GO

ALTER TABLE [swapi].[CharacterEpisode]
	ADD CONSTRAINT UQ_CharacterEpisode_CharacterIdEpisodeId UNIQUE ([CharacterId],[Episode]);
GO