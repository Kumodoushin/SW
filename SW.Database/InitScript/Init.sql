DECLARE @luke NVARCHAR(400), @anakin NVARCHAR(400), @leia NVARCHAR(400), @han NVARCHAR(400), @tarkin NVARCHAR(400), @annoyance NVARCHAR(400), @hacker NVARCHAR(400);

SET @luke=N'Luke Skywalker';
SET @anakin = N'Darth Vader';
SET @leia = N'Leia Organa';
SET @han = N'Han Solo';
SET @tarkin = N'Wilhuff Tarkin';
SET @annoyance = N'C-3PO';
SET @hacker = N'R2-D2';

INSERT INTO [swapi].[Character] ([Name]) VALUES (@luke);
INSERT INTO [swapi].[Character] ([Name]) VALUES (@anakin);
INSERT INTO [swapi].[Character] ([Name]) VALUES (@han);
INSERT INTO [swapi].[Character] ([Name]) VALUES (@leia);
INSERT INTO [swapi].[Character] ([Name]) VALUES (@tarkin);
INSERT INTO [swapi].[Character] ([Name]) VALUES (@annoyance);
INSERT INTO [swapi].[Character] ([Name]) VALUES (@hacker);

INSERT INTO [swapi].[CharacterFriend] ([CharacterId],[FriendId])
SELECT c.[Id],f.[Id]
FROM [swapi].[Character] c
INNER JOIN [swapi].[Character] f ON f.[Name] in (@han,@leia,@annoyance,@hacker) and c.[Name]=@luke;

INSERT INTO [swapi].[CharacterFriend] ([CharacterId],[FriendId])
SELECT c.[Id],f.[Id]
FROM [swapi].[Character] c
INNER JOIN [swapi].[Character] f ON f.[Name] in (@tarkin) and c.[Name]=@anakin;

INSERT INTO [swapi].[CharacterFriend] ([CharacterId],[FriendId])
SELECT c.[Id],f.[Id]
FROM [swapi].[Character] c
INNER JOIN [swapi].[Character] f ON f.[Name] in (@luke,@leia,@hacker) and c.[Name]=@han;

INSERT INTO [swapi].[CharacterFriend] ([CharacterId],[FriendId])
SELECT c.[Id],f.[Id]
FROM [swapi].[Character] c
INNER JOIN [swapi].[Character] f ON f.[Name] in (@luke,@han,@annoyance,@hacker) and c.[Name]=@leia;

INSERT INTO [swapi].[CharacterFriend] ([CharacterId],[FriendId])
SELECT c.[Id],f.[Id]
FROM [swapi].[Character] c
INNER JOIN [swapi].[Character] f ON f.[Name] in (@anakin) and c.[Name]=@tarkin;

INSERT INTO [swapi].[CharacterFriend] ([CharacterId],[FriendId])
SELECT c.[Id],f.[Id]
FROM [swapi].[Character] c
INNER JOIN [swapi].[Character] f ON f.[Name] in (@luke,@han,@leia,@hacker) and c.[Name]=@annoyance;

INSERT INTO [swapi].[CharacterFriend] ([CharacterId],[FriendId])
SELECT c.[Id],f.[Id]
FROM [swapi].[Character] c
INNER JOIN [swapi].[Character] f ON f.[Name] in (@luke,@leia,@han) and c.[Name]=@hacker;

INSERT INTO [swapi].[CharacterEpisode] ([CharacterId],[Episode])
SELECT c.[Id],1 FROM [swapi].[Character] c
UNION ALL
SELECT c.[Id],2 FROM [swapi].[Character] c WHERE c.[Name]<>@tarkin
UNION ALL
SELECT c.[Id],3 FROM [swapi].[Character] c WHERE c.[Name]<>@tarkin
GO