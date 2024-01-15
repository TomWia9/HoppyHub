DECLARE @UserId          UNIQUEIDENTIFIER
DECLARE @OpinionsPerBeer INT

-- Number of opinions generating for beer
SET @OpinionsPerBeer = 20

-- Select Id for "user@localhost" user
SELECT @UserId = Id
FROM Users
WHERE UserName = 'user@localhost'

-- Generate @OpinionsPerBeer opinions for each beer
INSERT INTO Opinions (Id, Rating, Comment, ImageUri, BeerId, Created, CreatedBy, LastModified, LastModifiedBy)
SELECT NEWID(),
       FLOOR(RAND(CHECKSUM(NEWID())) * 10) + 1,
       'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed quam arcu, maximus eget augue quis, porttitor eleifend diam. Donec pharetra dictum tellus sodales fermentum. Aenean eget erat diam. Nullam ac nisi vehicula, viverra eros id, gravida mauris. Cras eget est in mi placerat pellentesque at et ex. Sed feugiat venenatis est, nec efficitur turpis volutpat at. Nulla sapien ex, condimentum eget justo eget, rutrum luctus turpis. Aliquam et justo ut dolor placerat porttitor a ut metus. Morbi suscipit erat quis tortor interdum facilisis. Maecenas quis interdum sem, vel pharetra ante. Etiam scelerisque lectus sit amet metus consequat, non consectetur felis dapibus. Integer sapien eros, gravida ut condimentum ut, euismod fermentum odio. Sed purus sapien, pellentesque eget luctus vitae, dictum quis enim. Sed ullamcorper, magna eget tempor dignissim, velit augue placerat mauris, eu dictum massa orci at ante. Vivamus convallis eros eu arcu congue, vel luctus ex porttitor.',
       'https://hoppyhub.blob.core.windows.net/hoppyhub-container/Beers/temp.jpg',
       Beers.Id,
       CreatedDate,
       @UserId,
       CreatedDate,
       @UserId
FROM Beers
         CROSS JOIN (SELECT TOP (@OpinionsPerBeer) DATEADD(DAY, ABS(CHECKSUM(NEWID())) %
                                                                (DATEDIFF(DAY, '2020-01-01', '2023-12-31') + 1),
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