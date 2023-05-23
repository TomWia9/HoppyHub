DECLARE @UserId          UNIQUEIDENTIFIER
DECLARE @OpinionsPerBeer INT

-- Number of opinions generating for beer
SET @OpinionsPerBeer = 20

-- Select Id for "user@localhost" user
SELECT @UserId = Id
FROM AspNetUsers
WHERE Username = 'user@localhost'

-- Generate @OpinionsPerBeer opinions for each beer
INSERT INTO Opinions (Id, Rate, Comment, BeerId, Created, CreatedBy, LastModified, LastModifiedBy)
SELECT NEWID(),
       FLOOR(RAND(CHECKSUM(NEWID())) * 10) + 1,
       'Sample comment',
       Beers.Id,
       CreatedDate,
       @UserId,
       CreatedDate,
       @UserId
FROM Beers
         CROSS JOIN (SELECT TOP (@OpinionsPerBeer) DATEADD(DAY, ABS(CHECKSUM(NEWID())) %
                                                                (DATEDIFF(DAY, '2020-01-01', '2023-12-31') + 1),
                                                           '2020-01-01') AS CreatedDate
                     FROM sys.all_columns) AS Opinions
ORDER BY Beers.Id