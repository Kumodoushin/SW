﻿CREATE TABLE [swapi].[Character]
(
	[Id] UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID() NOT NULL,
	[Name] NVARCHAR(400) NOT NULL,
);
GO

ALTER TABLE [swapi].[Character]
	ADD CONSTRAINT PK_Character PRIMARY KEY CLUSTERED([Id]);
GO

ALTER TABLE [swapi].[Character]
	ADD CONSTRAINT UQ_Character UNIQUE([Name]);
GO