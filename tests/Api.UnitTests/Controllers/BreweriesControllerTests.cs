using Api.Controllers;
using Application.Breweries.Commands.CreateBrewery;
using Application.Breweries.Commands.DeleteBrewery;
using Application.Breweries.Commands.UpdateBrewery;
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
        var expectedResult = new BreweryDto { Id = breweryId, Name = "Test brewery" };

        MediatorMock.Setup(m => m.Send(It.IsAny<GetBreweryQuery>(), CancellationToken.None))
            .ReturnsAsync(expectedResult);

        // Act
        var response = await Controller.GetBrewery(breweryId);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(expectedResult);
    }

    /// <summary>
    ///     Tests that CreateBrewery method returns CreatedAtAction.
    /// </summary>
    [Fact]
    public async Task CreateBrewery_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var command = new CreateBreweryCommand();
        var expectedResult = new BreweryDto { Id = Guid.NewGuid() };
        MediatorMock.Setup(m => m.Send(command, CancellationToken.None)).ReturnsAsync(expectedResult);

        // Act
        var response = await Controller.CreateBrewery(command);

        // Assert
        response.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = (CreatedAtActionResult)response.Result!;

        createdAtActionResult.ActionName.Should().Be("GetBrewery");
        createdAtActionResult.RouteValues.Should().NotBeNull();
        createdAtActionResult.RouteValues!["id"].Should().Be(expectedResult.Id);
        createdAtActionResult.Value.Should().Be(expectedResult);
    }

    /// <summary>
    ///     Tests that UpdateBrewery method returns NoContent when Id is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBrewery_ShouldReturnNoContent_WhenIdIsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new UpdateBreweryCommand { Id = id };
        MediatorMock.Setup(m => m.Send(command, CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var response = await Controller.UpdateBrewery(id, command);

        // Assert
        response.Should().BeOfType<NoContentResult>();
    }

    /// <summary>
    ///     Tests that UpdateBrewery method returns BadRequest when id is invalid.
    /// </summary>
    [Fact]
    public async Task UpdateBrewery_ShouldReturnBadRequest_WhenIdIsInvalid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new UpdateBreweryCommand { Id = Guid.NewGuid() };

        // Act
        var response = await Controller.UpdateBrewery(id, command);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be(ExpectedInvalidIdMessage);
    }

    /// <summary>
    ///     Tests that DeleteBrewery method returns NoContent.
    /// </summary>
    [Fact]
    public async Task DeleteBrewery_ShouldReturnNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        MediatorMock.Setup(m => m.Send(It.IsAny<DeleteBreweryCommand>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        // Act
        var response = await Controller.DeleteBrewery(id);

        // Assert
        response.Should().BeOfType<NoContentResult>();
    }
}