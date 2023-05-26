using Api.Controllers;
using Application.Favorites.Commands.CreateFavorite;
using Application.Favorites.Commands.DeleteFavorite;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Api.UnitTests.Controllers;

/// <summary>
///     Unit tests for the <see cref="FavoritesController"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class FavoritesControllerTests : ControllerSetup<FavoritesController>
{
    /// <summary>
    ///     Tests that CreateFavorite method returns NoContent.
    /// </summary>
    [Fact]
    public async Task CreateFavorite_ShouldReturnNoContent()
    {
        // Arrange
        var command = new CreateFavoriteCommand { BeerId = Guid.NewGuid() };
        MediatorMock.Setup(m => m.Send(command, CancellationToken.None)).Returns(Task.CompletedTask);

        // Act
        var response = await Controller.CreateFavorite(command);

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
        var id = Guid.NewGuid();
        MediatorMock.Setup(m => m.Send(It.IsAny<DeleteFavoriteCommand>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        // Act
        var response = await Controller.DeleteFavorite(id);

        // Assert
        response.Should().BeOfType<NoContentResult>();
    }
}