using Application.Beers.Commands.UpdateBeer;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.Beers.Commands.UpdateBeer;

/// <summary>
///     Unit tests for the <see cref="UpdateBeerCommandHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateBeerCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly UpdateBeerCommandHandler _handler;

    /// <summary>
    ///     Setups UpdateBeerCommandHandlerTests.
    /// </summary>
    public UpdateBeerCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new UpdateBeerCommandHandler(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method updates beer when beer exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldUpdateBeer_WhenBeerExists()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var existingBeer = new Beer { Id = beerId, Name = "Old Name" };
        _contextMock.Setup(x => x.Beers.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBeer);

        var command = new UpdateBeerCommand { Id = beerId, Name = "New Name" };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when beer does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBeerDoesNotExist()
    {
        // Arrange
        _contextMock.Setup(x => x.Beers.FindAsync(new object[] { 1 }, CancellationToken.None))
            .ReturnsAsync((Beer?)null);

        var command = new UpdateBeerCommand { Id = Guid.NewGuid(), Name = "New Name" };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}