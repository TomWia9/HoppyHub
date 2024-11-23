DECLARE @UserId          UNIQUEIDENTIFIER
DECLARE @OpinionsPerBeer INT
DECLARE @ImageUri VARCHAR(100)

-- Number of opinions generating for beer
SET @OpinionsPerBeer = 20

-- Image uri
SET @ImageUri = 'https://hoppyhub.blob.core.windows.net/hoppyhub/Beers/temp.jpg'

-- Select Id for "user@localhost" user
SELECT @UserId = Id
FROM Users
WHERE UserName = 'user@localhost'

-- Generate @OpinionsPerBeer opinions for each beer
INSERT INTO Opinions (Id, Rating, Comment, ImageUri, BeerId, Created, CreatedBy, LastModified, LastModifiedBy)
SELECT NEWID(),
       FLOOR(RAND(CHECKSUM(NEWID())) * 10) + 1,
       'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque ultrices dui vel augue consequat, eu hendrerit tellus tincidunt. Quisque molestie ligula ac turpis feugiat consequat. Morbi dictum augue turpis, sit amet rhoncus odio suscipit nec. Sed tempor ligula at erat posuere pharetra morbi.',
       @ImageUri,
       Beers.Id,
       CreatedDate,
       @UserId,
       CreatedDate,
       @UserId
FROM Beers
         CROSS JOIN (SELECT TOP (@OpinionsPerBeer) DATEADD(DAY, ABS(CHECKSUM(NEWID())) %
                                                                (DATEDIFF(DAY, '2020-01-01', GETDATE()) + 1),
                                                           '2020-01-01') AS CreatedDate
                     FROM sys.all_columns) AS BeerOpinions
ORDER BY Beers.Id

-- Update beers ratings and opinions count in BeerManagement database
UPDATE BeerManagement.dbo.Beers
SET Rating        = ISNULL(Opinions.AverageRating, 0),
    OpinionsCount = @OpinionsPerBeer
FROM BeerManagement.dbo.Beers
         LEFT JOIN (SELECT BeerId, ROUND(AVG(CAST(Rating AS FLOAT)), 2) AS AverageRating
                    FROM Opinions
                    GROUP BY BeerId) AS Opinions ON BeerManagement.dbo.Beers.Id = Opinions.BeerId