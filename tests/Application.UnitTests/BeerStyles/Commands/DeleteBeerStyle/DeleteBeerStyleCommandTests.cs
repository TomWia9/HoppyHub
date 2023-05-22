using Application.BeerStyles.Commands.DeleteBeerStyle;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.BeerStyles.Commands.DeleteBeerStyle;

/// <summary>
///     Unit tests for the <see cref="DeleteBeerStyleCommand"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class DeleteBeerStyleCommandTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly DeleteBeerStyleCommandHandler _handler;

    /// <summary>
    ///     Setups DeleteBeerStyleCommandTests.
    /// </summary>
    public DeleteBeerStyleCommandTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new DeleteBeerStyleCommandHandler(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method removes beer style from database when beer style exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldRemoveBeerStyleFromDatabase_WhenBeerStyleExists()
    {
        // Arrange
        var beerStyleId = Guid.NewGuid();
        var beerStyle = new BeerStyle { Id = beerStyleId };
        _contextMock.Setup(x => x.BeerStyles.FindAsync(new object[] { beerStyleId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beerStyle);
        var command = new DeleteBeerStyleCommand { Id = beerStyleId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.BeerStyles.Remove(beerStyle), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when beer style does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBeerStyleDoesNotExist()
    {
        // Arrange
        var beerStyleId = Guid.NewGuid();
        _contextMock.Setup(x => x.BeerStyles.FindAsync(new object[] { beerStyleId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BeerStyle?)null);
        var command = new DeleteBeerStyleCommand { Id = beerStyleId };

        // Act
        var action = new Func<Task>(() => _handler.Handle(command, CancellationToken.None));

        // Assert
        await action.Should().ThrowAsync<NotFoundException>();
        _contextMock.Verify(x => x.BeerStyles.Remove(It.IsAny<BeerStyle>()), Times.Never);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}