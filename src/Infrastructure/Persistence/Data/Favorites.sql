DECLARE @UserId UNIQUEIDENTIFIER

-- Select Id for "user@localhost" user
SELECT @UserId = Id
FROM AspNetUsers
WHERE Username = 'user@localhost'

-- Generate 10 favorites adds for user@localhost user.
INSERT INTO Favorites (Id, BeerId, Created, CreatedBy, LastModified, LastModifiedBy)
SELECT NEWID(), Id, RandomDate, @UserId, RandomDate, @UserId
FROM (SELECT TOP 10 Id, DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 1460, '2020-01-01') AS RandomDate
      FROM Beers
      ORDER BY NEWID()) AS RandomBeers;