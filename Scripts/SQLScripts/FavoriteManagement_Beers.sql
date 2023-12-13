INSERT INTO Beers (Id, Name, BreweryName, BreweryId)
SELECT Beers.Id,
       Beers.Name,
       Breweries.Name,
       Breweries.Id
FROM BeerManagement.dbo.Beers
         INNER JOIN BeerManagement.dbo.Breweries ON Beers.BreweryId = Breweries.Id
ORDER BY Beers.Id