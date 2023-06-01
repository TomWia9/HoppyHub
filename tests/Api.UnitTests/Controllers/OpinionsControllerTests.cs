using Api.Controllers;
using Application.Common.Models;
using Application.Opinions.Commands.CreateOpinion;
using Application.Opinions.Commands.DeleteOpinion;
using Application.Opinions.Commands.UpdateOpinion;
using Application.Opinions.Dtos;
using Application.Opinions.Queries.GetOpinion;
using Application.Opinions.Queries.GetOpinions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Api.UnitTests.Controllers;

/// <summary>
///     Unit tests for the <see cref="OpinionsController"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class OpinionsControllerTests : ControllerSetup<OpinionsController>
{
    /// <summary>
    ///     Tests that GetOpinions method returns PaginatedList of OpinionDto.
    /// </summary>
    [Fact]
    public async Task GetOpinions_ShouldReturnPaginatedListOfOpinionDto()
    {
        // Arrange
        var opinions = new List<OpinionDto>
        {
            new() { Id = Guid.NewGuid(), Rating = 9 },
            new() { Id = Guid.NewGuid(), Rating = 8 }
        };
        var query = new GetOpinionsQuery();
        var expectedResult = PaginatedList<OpinionDto>.Create(opinions, 1, 10);

        MediatorMock.Setup(m => m.Send(query, CancellationToken.None)).ReturnsAsync(expectedResult);

        // Act
        var response = await Controller.GetOpinions(query);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(expectedResult);
        Controller.Response.Headers.Should().ContainKey("X-Pagination");
        Controller.Response.Headers["X-Pagination"].Should().BeEquivalentTo(expectedResult.GetMetadata());
    }

    /// <summary>
    ///     Tests that GetOpinion method returns OpinionDto.
    /// </summary>
    [Fact]
    public async Task GetOpinion_ShouldReturnOpinionDto()
    {
        // Arrange
        var opinionId = Guid.NewGuid();
        var expectedResult = new OpinionDto { Id = opinionId, Rating = 3 };

        MediatorMock.Setup(m => m.Send(It.IsAny<GetOpinionQuery>(), CancellationToken.None))
            .ReturnsAsync(expectedResult);

        // Act
        var response = await Controller.GetOpinion(opinionId);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(expectedResult);
    }
    
    /// <summary>
    ///     Tests that CreateOpinion method returns CreatedAtAction.
    /// </summary>
    [Fact]
    public async Task CreateOpinion_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var command = new CreateOpinionCommand();
        var expectedResult = new OpinionDto { Id = Guid.NewGuid() };
        MediatorMock.Setup(m => m.Send(command, CancellationToken.None)).ReturnsAsync(expectedResult);

        // Act
        var response = await Controller.CreateOpinion(command);

        // Assert
        response.Result.Should().BeOfType<CreatedAtActionResult>();
        var createdAtActionResult = (CreatedAtActionResult)response.Result!;

        createdAtActionResult.ActionName.Should().Be("GetOpinion");
        createdAtActionResult.RouteValues.Should().NotBeNull();
        createdAtActionResult.RouteValues!["id"].Should().Be(expectedResult.Id);
        createdAtActionResult.Value.Should().Be(expectedResult);
    }

    /// <summary>
    ///     Tests that UpdateOpinion method returns NoContent when Id is valid.
    /// </summary>
    [Fact]
    public async Task UpdateOpinion_ShouldReturnNoContent_WhenIdIsValid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new UpdateOpinionCommand { Id = id };
        MediatorMock.Setup(m => m.Send(command, CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var response = await Controller.UpdateOpinion(id, command);

        // Assert
        response.Should().BeOfType<NoContentResult>();
    }

    /// <summary>
    ///     Tests that UpdateOpinion method returns BadRequest when id is invalid.
    /// </summary>
    [Fact]
    public async Task UpdateOpinion_ShouldReturnBadRequest_WhenIdIsInvalid()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new UpdateOpinionCommand { Id = Guid.NewGuid() };

        // Act
        var response = await Controller.UpdateOpinion(id, command);

        // Assert
        response.Should().BeOfType<BadRequestObjectResult>().Which.Value.Should().Be(ExpectedInvalidIdMessage);
    }

    /// <summary>
    ///     Tests that DeleteOpinion method returns NoContent.
    /// </summary>
    [Fact]
    public async Task DeleteOpinion_ShouldReturnNoContent()
    {
        // Arrange
        var id = Guid.NewGuid();
        MediatorMock.Setup(m => m.Send(It.IsAny<DeleteOpinionCommand>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        // Act
        var response = await Controller.DeleteOpinion(id);

        // Assert
        response.Should().BeOfType<NoContentResult>();
    }
}