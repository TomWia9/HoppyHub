INSERT INTO Beers (Id, Name, BreweryId, BreweryName)
SELECT Beers.Id,
       Beers.Name,
       Beers.BreweryId,
       Breweries.Name
FROM BeerManagement.dbo.Beers
INNER JOIN BeerManagement.dbo.Breweries ON Beers.BreweryId = Breweries.Id
ORDER BY Beers.Id