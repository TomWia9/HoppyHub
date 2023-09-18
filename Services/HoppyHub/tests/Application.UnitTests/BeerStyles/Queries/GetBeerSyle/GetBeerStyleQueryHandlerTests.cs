using Application.BeerStyles.Queries.GetBeerStyle;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.BeerStyles.Queries.GetBeerSyle;

/// <summary>
///     Tests for the <see cref="GetBeerStyleQueryHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetBeerStyleQueryHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly GetBeerStyleQueryHandler _handler;

    /// <summary>
    ///     Setups GetBeerStyleQueryHandlerTests.
    /// </summary>
    public GetBeerStyleQueryHandlerTests()
    {
        var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        var mapper = configurationProvider.CreateMapper();

        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new GetBeerStyleQueryHandler(_contextMock.Object, mapper);
    }

    /// <summary>
    ///     Tests that Handle method returns BeerStyleDto when Id is valid.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnBeerStyleDto_WhenIdIsValid()
    {
        // Arrange
        var beerStyleId = Guid.NewGuid();
        var beerStyle = new BeerStyle
            { Id = beerStyleId, Name = "IPA", Description = "test description", CountryOfOrigin = "USA" };
        _contextMock.Setup(x => x.BeerStyles.FindAsync(new object[] { beerStyleId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beerStyle);

        var query = new GetBeerStyleQuery { Id = beerStyleId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(beerStyle.Id);
        result.Name.Should().Be(beerStyle.Name);
        result.Description.Should().Be(beerStyle.Description);
        result.CountryOfOrigin.Should().Be(beerStyle.CountryOfOrigin);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when Id is invalid.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenIdIsInvalid()
    {
        // Arrange
        _contextMock
            .Setup(x => x.BeerStyles.FindAsync(new object[] { It.IsAny<Guid>() }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BeerStyle?)null);
        var query = new GetBeerStyleQuery { Id = Guid.NewGuid() };

        // Act & Assert
        await _handler.Invoking(x => x.Handle(query, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }
}