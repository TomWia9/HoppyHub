using Api.Controllers;
using Application.Breweries.Dtos;
using Application.Breweries.Queries.GetBreweries;
using Application.Breweries.Queries.GetBrewery;
using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Api.UnitTests.Controllers;

/// <summary>
///     Unit tests for the <see cref="BreweriesController"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BreweriesControllerTests : ControllerSetup<BreweriesController>
{
    /// <summary>
    ///     Tests that GetBreweries method returns PaginatedList of BreweriesDto.
    /// </summary>
    [Fact]
    public async Task GetBreweries_ShouldReturnPaginatedListOfBreweriesDto()
    {
        // Arrange
        var breweries = new List<BreweryDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Brewery 1" },
            new() { Id = Guid.NewGuid(), Name = "Brewery 2" }
        };
        var query = new GetBreweriesQuery();
        var expectedResult = PaginatedList<BreweryDto>.Create(breweries, 1, 10);

        MediatorMock.Setup(m => m.Send(query, CancellationToken.None)).ReturnsAsync(expectedResult);

        // Act
        var response = await Controller.GetBreweries(query);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(expectedResult);
        Controller.Response.Headers.Should().ContainKey("X-Pagination");
        Controller.Response.Headers["X-Pagination"].Should().BeEquivalentTo(expectedResult.GetMetadata());
    }

    /// <summary>
    ///     Tests that GetBrewery method returns BreweryDto.
    /// </summary>
    [Fact]
    public async Task GetBrewery_ShouldReturnBreweryDto()
    {
        // Arrange
        var breweryId = Guid.NewGuid();
        var expectedResult = new BreweryDto() { Id = breweryId, Name = "Test brewery" };

        MediatorMock.Setup(m => m.Send(It.IsAny<GetBreweryQuery>(), CancellationToken.None))
            .ReturnsAsync(expectedResult);

        // Act
        var response = await Controller.GetBrewery(breweryId);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(expectedResult);
    }
}