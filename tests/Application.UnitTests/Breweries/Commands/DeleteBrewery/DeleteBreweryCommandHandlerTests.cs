using Application.Breweries.Commands.DeleteBrewery;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.Breweries.Commands.DeleteBrewery;

/// <summary>
///     Unit tests for the <see cref="DeleteBreweryCommandHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class DeleteBreweryCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly DeleteBreweryCommandHandler _handler;

    /// <summary>
    ///     Setups DeleteBreweryCommandHandlerTests.
    /// </summary>
    public DeleteBreweryCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new DeleteBreweryCommandHandler(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method removes Brewery from database when Brewery exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldRemoveBreweryFromDatabase_WhenBreweryExists()
    {
        // Arrange
        var breweryId = Guid.NewGuid();
        var brewery = new Brewery { Id = breweryId };
        _contextMock.Setup(x => x.Breweries.FindAsync(new object[] { breweryId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(brewery);
        var command = new DeleteBreweryCommand { Id = breweryId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.Breweries.Remove(brewery), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when Brewery does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBreweryDoesNotExist()
    {
        // Arrange
        var breweryId = Guid.NewGuid();
        _contextMock.Setup(x => x.Breweries.FindAsync(new object[] { breweryId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Brewery?)null);
        var command = new DeleteBreweryCommand { Id = breweryId };

        // Act
        var action = new Func<Task>(() => _handler.Handle(command, CancellationToken.None));

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
        _contextMock.Verify(x => x.Breweries.Remove(It.IsAny<Brewery>()), Times.Never);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}