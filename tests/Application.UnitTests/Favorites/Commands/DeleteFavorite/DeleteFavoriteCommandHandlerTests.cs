using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Favorites.Commands.DeleteFavorite;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Favorites.Commands.DeleteFavorite;

/// <summary>
///     Unit tests for the <see cref="DeleteFavoriteCommandHandler" /> class.
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
    ///     Tests that Handle method removes beer from favorites when favorite beer exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldRemoveFavoriteBeerFromDatabase_WhenFavoriteBeerExists()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var favorite = new Favorite { Id = Guid.NewGuid(), BeerId = beerId, CreatedBy = userId };
        var favoritesDbSetMock = new List<Favorite> { favorite }.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(x => x.Favorites).Returns(favoritesDbSetMock.Object);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        var command = new DeleteFavoriteCommand { BeerId = beerId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.Favorites.Remove(favorite), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when beer does not exists in favorites.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBeerDoesNotExistsInFavorites()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var favoritesDbSetMock = Enumerable.Empty<Favorite>().AsQueryable().BuildMockDbSet();
        _contextMock.Setup(x => x.Favorites).Returns(favoritesDbSetMock.Object);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        var command = new DeleteFavoriteCommand { BeerId = beerId };
        var expectedMessage =
            $"Beer with \"{beerId}\" id has been not found in favorites of user with \"{userId}\" id.";

        // Act
        var action = new Func<Task>(() => _handler.Handle(command, CancellationToken.None));

        // Assert
        await action.Should().ThrowAsync<NotFoundException>().WithMessage(expectedMessage);
        _contextMock.Verify(x => x.Favorites.Remove(It.IsAny<Favorite>()), Times.Never);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}