
CREATE VIEW [swapi].[Friend_uvw]
AS
SELECT c.[Id] as "CharacterId",f.[Id] as "FriendId",f.[Name] as "FriendName"
FROM [swapi].[Character] c 
	INNER JOIN [swapi].[CharacterFriend] cf ON c.[Id] = cf.[CharacterId]
	INNER JOIN [swapi].[Character] f ON cf.[FriendId] = f.[Id];

GO