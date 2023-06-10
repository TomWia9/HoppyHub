using Application.Beers.Commands.DeleteBeer;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.Beers.Commands.DeleteBeer;

/// <summary>
///     Unit tests for the <see cref="DeleteBeerCommandHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class DeleteBeerCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The azure storage service mock.
    /// </summary>
    private readonly Mock<IAzureStorageService> _azureStorageServiceMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly DeleteBeerCommandHandler _handler;

    /// <summary>
    ///     Setups DeleteBeerCommandHandlerTests.
    /// </summary>
    public DeleteBeerCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _azureStorageServiceMock = new Mock<IAzureStorageService>();
        _handler = new DeleteBeerCommandHandler(_contextMock.Object, _azureStorageServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method removes beer from database when beer exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldRemoveBeerFromDatabase_WhenBeerExists()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var beer = new Beer { Id = beerId };
        _contextMock.Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        var command = new DeleteBeerCommand { Id = beerId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.Beers.Remove(beer), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when beer does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBeerDoesNotExist()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        _contextMock.Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Beer?)null);
        var command = new DeleteBeerCommand { Id = beerId };

        // Act
        var action = new Func<Task>(() => _handler.Handle(command, CancellationToken.None));

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
        _contextMock.Verify(x => x.Beers.Remove(It.IsAny<Beer>()), Times.Never);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}