DECLARE @UserId          UNIQUEIDENTIFIER

-- Select Id for "user@localhost" user
SELECT @UserId = Id
FROM Users
WHERE UserName = 'user@localhost'

-- Generate 10 favorites adds for user@localhost user.
INSERT INTO Favorites (Id, BeerId, Created, CreatedBy, LastModified, LastModifiedBy)
SELECT NEWID(), Id, RandomDate, @UserId, RandomDate, @UserId
FROM (SELECT TOP 10 Id, DATEADD(DAY, ABS(CHECKSUM(NEWID())) % 1460, '2020-01-01') AS RandomDate
      FROM Beers
      ORDER BY NEWID()) AS RandomBeers;

-- Update favorites count in BeerManagement database
UPDATE BeerManagement.dbo.Beers
SET FavoritesCount = (
    SELECT COUNT(*)
    FROM Favorites
    WHERE BeerId = BeerManagement.dbo.Beers.Id
)