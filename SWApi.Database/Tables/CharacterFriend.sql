
CREATE TABLE [swapi].[CharacterFriend]
(
	[CharacterId] UNIQUEIDENTIFIER NOT NULL,
	[FriendId] UNIQUEIDENTIFIER NOT NULL,
);
GO

ALTER TABLE [swapi].[CharacterFriend]
	ADD CONSTRAINT PK_CharacterFriend PRIMARY KEY CLUSTERED([CharacterId],[FriendId]);
GO

ALTER TABLE [swapi].[CharacterFriend]
	ADD CONSTRAINT FK_CharacterFriend_CharacterId FOREIGN KEY ([CharacterId]) REFERENCES [swapi].[Character]([Id]);
GO

ALTER TABLE [swapi].[CharacterFriend]
	ADD CONSTRAINT FK_CharacterFriend_FriendId FOREIGN KEY ([FriendId]) REFERENCES [swapi].[Character]([Id]);
GO

ALTER TABLE [swapi].[CharacterFriend]
	ADD CONSTRAINT CK_CharacterFriend_DifferentCharacterAndFriend CHECK ([CharacterId]<>[FriendId]);
GO
