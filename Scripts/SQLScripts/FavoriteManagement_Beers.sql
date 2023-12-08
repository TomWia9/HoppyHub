INSERT INTO Beers (Id, Name, BreweryName)
SELECT Beers.Id,
       Beers.Name,
       Breweries.Name
FROM BeerManagement.dbo.Beers
INNER JOIN BeerManagement.dbo.Breweries ON Beers.BreweryId = Breweries.Id
ORDER BY Beers.Id