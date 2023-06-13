using Api.Controllers;
using Application.BeerImages.Commands.DeleteBeerImage;
using Application.BeerImages.Commands.UpsertBeerImage;
using Application.Beers.Commands.CreateBeer;
using Application.Beers.Commands.DeleteBeer;
using Application.Beers.Commands.UpdateBeer;
using Application.Beers.Dtos;
using Application.Beers.Queries.GetBeer;
using Application.Beers.Queries.GetBeers;
using Application.Common.Models;
using Application.Favorites.Commands.CreateFavorite;
using Application.Favorites.Commands.DeleteFavorite;
using Application.Favorites.Queries.GetFavorites;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Api.UnitTests.Controllers;

/// <summary>
///     Unit tests for the <see cref="BeersController"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeersControllerTests : ControllerSetup<BeersController>
{
    /// <summary>
    ///     Tests that GetBeers method returns PaginatedList of BeerDto.
    /// </summary>
    [Fact]
    public async Task GetBeers_ShouldReturnPaginatedListOfBeerDto()
    {
        // Arrange
        var beers = new List<BeerDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Beer 1" },
            new() { Id = Guid.NewGuid(), Name = "Beer 2" }
        };
        var query = new GetBeersQuery();
        var expectedResult = PaginatedList<BeerDto>.Create(beers, 1, 10);

        MediatorMock.Setup(m => m.Send(query, CancellationToken.None)).ReturnsAsync(expectedResult);

        // Act
        var response = await Controller.GetBeers(query);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(expectedResult);
        Controller.Response.Headers.Should().ContainKey("X-Pagination");
        Controller.Response.Headers["X-Pagination"].Should().BeEquivalentTo(expectedResult.GetMetadata());
    }

    /// <summary>
    ///     Tests that GetBeer method returns BeerDto.
    /// </summary>
    [Fact]
    public async Task GetBeer_ShouldReturnBeerDto()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var expectedResult = new BeerDto { Id = beerId, Name = "Test Beer" };

        MediatorMock.Setup(m => m.Send(It.IsAny<GetBeerQuery>(), CancellationToken.None)).ReturnsAsync(expectedResult);

        // Act
        var response = await Controller.GetBeer(beerId);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(expectedResult);
    }

    /// <summary>
    ///     Tests that CreateBeer method returns CreatedAtAction.
    /// </summary>
    [Fact]
    public async Task CreateBeer_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var command = new CreateBeerCommand();
        var expectedResult = new BeerDto { Id = Guid.NewGuid() };
        MediatorMock.Setup(m => m.Send(command, CancellationToken.None)).ReturnsAsync(expectedResult);

        // Act
        var response = await Controller.CreateBeer(command);

        // Assert
        response.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = (CreatedAtActionResult)response.Result!;

        createdAtActionResult.ActionName.Should().Be("GetBeer");
        createdAtActionResult.RouteValues.Should().NotBeNull();
        createdAtActionResult.RouteValues!["id"].Should().Be(expectedResult.Id);
        createdAtActionResult.Value.Should().Be(expectedResult);
    }

    /// <summary>
    ///     Tests that UpdateBeer method returns NoContent when Id is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBeer_ShouldReturnNoContent_WhenIdIsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new UpdateBeerCommand { Id = id };
        MediatorMock.Setup(m => m.Send(command, CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var response = await Controller.UpdateBeer(id, command);

        // Assert
        response.Should().BeOfType<NoContentResult>();
    }

    /// <summary>
    ///     Tests that UpdateBeer method returns BadRequest when id is invalid.
    /// </summary>
    [Fact]
    public async Task UpdateBeer_ShouldReturnBadRequest_WhenIdIsInvalid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new UpdateBeerCommand { Id = Guid.NewGuid() };

        // Act
        var response = await Controller.UpdateBeer(id, command);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be(ExpectedInvalidIdMessage);
    }

    /// <summary>
    ///     Tests that DeleteBeer method returns NoContent.
    /// </summary>
    [Fact]
    public async Task DeleteBeer_ShouldReturnNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        MediatorMock.Setup(m => m.Send(It.IsAny<DeleteBeerCommand>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        // Act
        var response = await Controller.DeleteBeer(id);

        // Assert
        response.Should().BeOfType<NoContentResult>();
    }

    /// <summary>
    ///     Tests that UpsertBeerImage method returns NoContent when beer id is valid.
    /// </summary>
    [Fact]
    public async Task UpsertBeerImage_ShouldReturnNoContent_WhenBeerIdIsValid()
    {
        // Arrange
        const string imageUri = "https://test.com/test.jpg";
        var beerId = Guid.NewGuid();
        var command = new UpsertBeerImageCommand { BeerId = beerId };
        MediatorMock.Setup(m => m.Send(command, CancellationToken.None)).ReturnsAsync(imageUri);

        // Act
        var response = await Controller.UpsertBeerImage(beerId, command);

        // Assert
        response.Should().BeOfType<CreatedAtActionResult>();
    }

    /// <summary>
    ///     Tests that UpsertBeerImage method returns BadRequest when beer id is invalid.
    /// </summary>
    [Fact]
    public async Task UpsertBeerImage_ShouldReturnBadRequest_WhenBeerIdIsInvalid()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var command = new UpsertBeerImageCommand { BeerId = Guid.NewGuid() };

        // Act
        var response = await Controller.UpsertBeerImage(beerId, command);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be(ExpectedInvalidIdMessage);
    }

    /// <summary>
    ///     Tests that DeleteBeerImage method returns NoContent.
    /// </summary>
    [Fact]
    public async Task DeleteBeerImage_ShouldReturnNoContent()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        MediatorMock.Setup(m => m.Send(It.IsAny<DeleteBeerImageCommand>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        // Act
        var response = await Controller.DeleteBeerImage(beerId);

        // Assert
        response.Should().BeOfType<NoContentResult>();
    }

    /// <summary>
    ///     Tests that GetFavorites method returns PaginatedList of BeerDto.
    /// </summary>
    [Fact]
    public async Task GetFavorites_ShouldReturnPaginatedListOfBeerDto()
    {
        // Arrange
        var favoriteBeers = new List<BeerDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Test beer 1",
                AlcoholByVolume = 6
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Test beer 2",
                AlcoholByVolume = 5
            }
        };
        var expectedResult = PaginatedList<BeerDto>.Create(favoriteBeers, 1, 10);
        var query = new GetFavoritesQuery();

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