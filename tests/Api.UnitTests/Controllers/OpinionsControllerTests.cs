using Api.Controllers;
using Application.Common.Models;
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
            new() { Id = Guid.NewGuid(), Rate = 9 },
            new() { Id = Guid.NewGuid(), Rate = 8 }
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
        var expectedResult = new OpinionDto { Id = opinionId, Rate = 3 };

        MediatorMock.Setup(m => m.Send(It.IsAny<GetOpinionQuery>(), CancellationToken.None))
            .ReturnsAsync(expectedResult);

        // Act
        var response = await Controller.GetOpinion(opinionId);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeSameAs(expectedResult);
    }
}