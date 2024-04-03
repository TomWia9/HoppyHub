INSERT INTO Beers (Id, Name, BreweryId)
SELECT Beers.Id,
       Beers.Name,
       Beers.BreweryId
FROM BeerManagement.dbo.Beers
ORDER BY Beers.Id