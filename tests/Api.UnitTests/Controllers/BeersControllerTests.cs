using Api.Controllers;
using Application.Beers;
using Application.Beers.Commands.CreateBeer;
using Application.Beers.Commands.DeleteBeer;
using Application.Beers.Commands.UpdateBeer;
using Application.Beers.Queries.GetBeer;
using Application.Beers.Queries.GetBeers;
using Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Api.UnitTests.Controllers;

/// <summary>
///     Unit tests for the <see cref="BeersControllerTests"/> class.
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
    public async Task CreateBeer_ShouldReturn_CreatedAtAction()
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
        response.Result.Should().BeOfType<NoContentResult>();
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
        response.Result.Should().BeOfType<BadRequestResult>();
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
        response.Result.Should().BeOfType<NoContentResult>();
    }
}