INSERT INTO Beers (Id, Name, BreweryId)
SELECT Beers.Id,
       Beers.Name,
       Breweries.Id
FROM BeerManagement.dbo.Beers
ORDER BY Beers.Id