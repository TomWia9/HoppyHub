using Application.Beers.Services;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Beers.Services;

/// <summary>
///     Unit tests for the <see cref="BeersService"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeersServiceTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The beers service.
    /// </summary>
    private readonly IBeersService _beersService;

    /// <summary>
    ///     Setups BeersServiceTests.
    /// </summary>
    public BeersServiceTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _beersService = new BeersService(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that CalculateBeerRatingAsync method calculates beer rating.
    /// </summary>
    [Fact]
    public async Task CalculateBeerRatingAsync_ShouldCalculateBeerRating()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var beer = new Beer
        {
            Id = beerId,
            Rating = 0
        };

        var opinions = new List<Opinion>
        {
            new()
            {
                Id = Guid.NewGuid(),
                BeerId = beerId,
                Rating = 6
            },
            new()
            {
                Id = Guid.NewGuid(),
                BeerId = beerId,
                Rating = 5
            }
        };
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(x => x.Beers.FindAsync(beerId)).ReturnsAsync(beer);
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);

        // Act
        await _beersService.CalculateBeerRatingAsync(beerId);

        // Assert
        _contextMock.Verify(x => x.Beers.FindAsync(beerId), Times.Once);
    }

    /// <summary>
    ///     Tests that CalculateBeerRatingAsync method throws NotFound exception when beer is not found.
    /// </summary>
    [Fact]
    public async Task CalculateBeerRatingAsync_ShouldThrowNotFoundExceptionWhenBeerIsNotFound()
    {
        // Arrange
        _contextMock.Setup(x => x.Beers.FindAsync(It.IsAny<Guid>())).ReturnsAsync((Beer?)null);

        // Act & Assert
        await _beersService.Invoking(x => x.CalculateBeerRatingAsync(It.IsAny<Guid>()))
            .Should().ThrowAsync<NotFoundException>();
    }
}