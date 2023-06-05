using Application.BeerStyles.Commands.UpdateBeerStyle;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.BeerStyles.Commands.UpdateBeerStyle;

/// <summary>
///     Unit tests for the <see cref="UpdateBeerStyleCommandHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateBeerStyleCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly UpdateBeerStyleCommandHandler _handler;

    /// <summary>
    ///     Setups UpdateBeerStyleCommandHandlerTests.
    /// </summary>
    public UpdateBeerStyleCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new UpdateBeerStyleCommandHandler(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method updates beer style when beer style exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldUpdateBeerStyle_WhenBeerStyleExists()
    {
        // Arrange
        var beerStyleId = Guid.NewGuid();
        var existingBeerStyle = new BeerStyle
            { Id = beerStyleId, Name = "Grodziskie", Description = "Old desc", CountryOfOrigin = "England" };
        _contextMock.Setup(x => x.BeerStyles.FindAsync(new object[] { beerStyleId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBeerStyle);

        var command = new UpdateBeerStyleCommand
            { Id = beerStyleId, Name = "Grodziskie", Description = "New desc", CountryOfOrigin = "Poland" };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when beer style does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBeerStyleDoesNotExist()
    {
        // Arrange
        _contextMock
            .Setup(x => x.BeerStyles.FindAsync(new object[] { It.IsAny<Guid>() }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BeerStyle?)null);
        var command = new UpdateBeerStyleCommand { Id = Guid.NewGuid(), Name = "New Name" };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}