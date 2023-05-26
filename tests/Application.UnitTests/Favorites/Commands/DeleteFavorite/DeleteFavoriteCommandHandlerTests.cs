using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Favorites.Commands.DeleteFavorite;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.Favorites.Commands.DeleteFavorite;

/// <summary>
///     Unit tests for the <see cref="DeleteFavoriteCommandHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class DeleteFavoriteCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The current user service mock.
    /// </summary>
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly DeleteFavoriteCommandHandler _handler;

    /// <summary>
    ///     Setups DeleteFavoriteCommandHandlerTests.
    /// </summary>
    public DeleteFavoriteCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _handler = new DeleteFavoriteCommandHandler(_contextMock.Object, _currentUserServiceMock.Object);
    }
    
    /// <summary>
    ///     Tests that Handle method removes favorite from database when favorite exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldRemoveFavoriteFromDatabase_WhenFavoriteExists()
    {
        // Arrange
        var favoriteId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var favorite = new Favorite { Id = favoriteId, CreatedBy = userId };
        _contextMock.Setup(x => x.Favorites.FindAsync(new object[] { favoriteId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(favorite);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        var command = new DeleteFavoriteCommand { Id = favoriteId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.Favorites.Remove(favorite), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when favorite does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenFavoriteDoesNotExist()
    {
        // Arrange
        var favoriteId = Guid.NewGuid();
        _contextMock.Setup(x => x.Favorites.FindAsync(new object[] { It.IsAny<Guid>() }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Favorite?)null);
        var command = new DeleteFavoriteCommand { Id = favoriteId };
        var expectedMessage = $"Entity \"{nameof(Favorite)}\" ({favoriteId}) was not found.";

        // Act
        var action = new Func<Task>(() => _handler.Handle(command, CancellationToken.None));

        // Assert
        await action.Should().ThrowAsync<NotFoundException>().WithMessage(expectedMessage);
        _contextMock.Verify(x => x.Favorites.Remove(It.IsAny<Favorite>()), Times.Never);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    ///     Tests that Handle method throws ForbiddenException when user tries to delete not his favorite and user has no admin access.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowForbiddenException_WhenUserTriesToDeleteNotHisFavoriteAndUserHasNoAdminAccess()
    {
        // Arrange
        var favoriteId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingFavorite = new Favorite
            { Id = favoriteId, BeerId = Guid.NewGuid(), CreatedBy = userId };

        _contextMock.Setup(x => x.Favorites.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingFavorite);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());

        var command = new DeleteFavoriteCommand
        {
            Id = favoriteId
        };

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<ForbiddenAccessException>();
    }

    /// <summary>
    ///      Tests that Handle method deletes favorite when user tries to delete not his favorite but he has admin access.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldDeleteFavorite_WhenUserTriesToDeleteNotHisFavoriteButHasAdminAccess()
    {
        // Arrange
        var favoriteId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingFavorite = new Favorite
            { Id = favoriteId, BeerId = Guid.NewGuid(), CreatedBy = userId };

        _contextMock.Setup(x => x.Favorites.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingFavorite);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(true);

        var command = new DeleteFavoriteCommand
        {
            Id = favoriteId
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}