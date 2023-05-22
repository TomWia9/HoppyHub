using Api.Controllers;
using Application.BeerStyles.Commands.CreateBeerStyle;
using Application.BeerStyles.Commands.DeleteBeerStyle;
using Application.BeerStyles.Commands.UpdateBeerStyle;
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

    /// <summary>
    ///     Tests that CreateBeerStyle method returns CreatedAtAction.
    /// </summary>
    [Fact]
    public async Task CreateBeerStyle_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var command = new CreateBeerStyleCommand();
        var expectedResult = new BeerStyleDto { Id = Guid.NewGuid() };
        MediatorMock.Setup(m => m.Send(command, CancellationToken.None)).ReturnsAsync(expectedResult);

        // Act
        var response = await Controller.CreateBeerStyle(command);

        // Assert
        response.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = (CreatedAtActionResult)response.Result!;

        createdAtActionResult.ActionName.Should().Be("GetBeerStyle");
        createdAtActionResult.RouteValues.Should().NotBeNull();
        createdAtActionResult.RouteValues!["id"].Should().Be(expectedResult.Id);
        createdAtActionResult.Value.Should().Be(expectedResult);
    }

    /// <summary>
    ///     Tests that UpdateBeerStyle method returns NoContent when Id is valid.
    /// </summary>
    [Fact]
    public async Task UpdateBeerStyle_ShouldReturnNoContent_WhenIdIsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new UpdateBeerStyleCommand { Id = id };
        MediatorMock.Setup(m => m.Send(command, CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var response = await Controller.UpdateBeerStyle(id, command);

        // Assert
        response.Should().BeOfType<NoContentResult>();
    }

    /// <summary>
    ///     Tests that UpdateBeerStyle method returns BadRequest when id is invalid.
    /// </summary>
    [Fact]
    public async Task UpdateBeerStyle_ShouldReturnBadRequest_WhenIdIsInvalid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new UpdateBeerStyleCommand { Id = Guid.NewGuid() };

        // Act
        var response = await Controller.UpdateBeerStyle(id, command);

        // Assert
        response.Should().BeOfType<BadRequestResult>();
    }

    /// <summary>
    ///     Tests that DeleteBeerStyle method returns NoContent.
    /// </summary>
    [Fact]
    public async Task DeleteBeerStyle_ShouldReturnNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        MediatorMock.Setup(m => m.Send(It.IsAny<DeleteBeerStyleCommand>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        // Act
        var response = await Controller.DeleteBeerStyle(id);

        // Assert
        response.Should().BeOfType<NoContentResult>();
    }
}