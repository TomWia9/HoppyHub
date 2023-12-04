using Api.Controllers;
using Application.Favorites.Commands.CreateFavorite;
using Application.Favorites.Commands.DeleteFavorite;
using Application.Favorites.Dtos;
using Application.Favorites.Queries.GetFavorites;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SharedUtilities.Models;

namespace Api.UnitTests.Controllers;

/// <summary>
///     Unit tests for the <see cref="FavoritesController" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class FavoritesControllerTests : ControllerSetup<FavoritesController>
{
    /// <summary>
    ///     Tests that GetFavorites method returns PaginatedList of BeerDto.
    /// </summary>
    [Fact]
    public async Task GetFavorites_ShouldReturnPaginatedListOfBeerDto()
    {
        // Arrange
        var beers = new List<BeerDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Beer 1" },
            new() { Id = Guid.NewGuid(), Name = "Beer 2" }
        };
        var query = new GetFavoritesQuery();
        var expectedResult = PaginatedList<BeerDto>.Create(beers, 1, 10);

        MediatorMock.Setup(m => m.Send(query, CancellationToken.None)).ReturnsAsync(expectedResult);

        // Act
        var response = await Controller.GetFavorites(query);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(expectedResult);
        Controller.Response.Headers.Should().ContainKey("X-Pagination");
        Controller.Response.Headers["X-Pagination"].Should().BeEquivalentTo(expectedResult.GetMetadata());
    }

    /// <summary>
    ///     Tests that CreateFavorite method returns NoContent.
    /// </summary>
    [Fact]
    public async Task CreateFavorite_ShouldReturnNoContent()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        MediatorMock.Setup(m => m.Send(It.IsAny<CreateFavoriteCommand>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        // Act
        var response = await Controller.CreateFavorite(beerId);

        // Assert
        response.Should().BeOfType<NoContentResult>();
    }

    /// <summary>
    ///     Tests that DeleteFavorite method returns NoContent.
    /// </summary>
    [Fact]
    public async Task DeleteFavorite_ShouldReturnNoContent()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        MediatorMock.Setup(m => m.Send(It.IsAny<DeleteFavoriteCommand>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        // Act
        var response = await Controller.DeleteFavorite(beerId);

        // Assert
        response.Should().BeOfType<NoContentResult>();
    }
}