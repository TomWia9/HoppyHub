using Api.Controllers;
using Application.BeerStyles.Dtos;
using Application.BeerStyles.Queries.GetBeerStyle;
using Application.BeerStyles.Queries.GetBeerStyles;
using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Api.UnitTests.Controllers;

/// <summary>
///     Unit tests for the <see cref="BeerStylesController"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeerStylesControllerTests : ControllerSetup<BeerStylesController>
{
    /// <summary>
    ///     Tests that GetBeerStyles method returns PaginatedList of BeerStyleDto.
    /// </summary>
    [Fact]
    public async Task GetBeerStyles_ShouldReturnPaginatedListOfBeerStyleDto()
    {
        // Arrange
        var beerStyles = new List<BeerStyleDto>
        {
            new() { Id = Guid.NewGuid(), Name = "IPA" },
            new() { Id = Guid.NewGuid(), Name = "APA" }
        };
        var query = new GetBeerStylesQuery();
        var expectedResult = PaginatedList<BeerStyleDto>.Create(beerStyles, 1, 10);

        MediatorMock.Setup(m => m.Send(query, CancellationToken.None)).ReturnsAsync(expectedResult);

        // Act
        var response = await Controller.GetBeerStyles(query);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(expectedResult);
        Controller.Response.Headers.Should().ContainKey("X-Pagination");
        Controller.Response.Headers["X-Pagination"].Should().BeEquivalentTo(expectedResult.GetMetadata());
    }

    /// <summary>
    ///     Tests that GetBeerStyle method returns BeerStyleDto.
    /// </summary>
    [Fact]
    public async Task GetBeerStyle_ShouldReturnBeerStylDto()
    {
        // Arrange
        var beerStyleId = Guid.NewGuid();
        var expectedResult = new BeerStyleDto { Id = beerStyleId, Name = "IPA" };

        MediatorMock.Setup(m => m.Send(It.IsAny<GetBeerStyleQuery>(), CancellationToken.None))
            .ReturnsAsync(expectedResult);

        // Act
        var response = await Controller.GetBeerStyle(beerStyleId);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(expectedResult);
    }
}