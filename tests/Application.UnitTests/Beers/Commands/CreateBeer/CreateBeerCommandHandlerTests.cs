using Application.Beers;
using Application.Beers.Commands.CreateBeer;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Beers.Commands.CreateBeer;

/// <summary>
///     Unit tests for the <see cref="CreateBeerCommandHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateBeerCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The mapper mock.
    /// </summary>
    private readonly Mock<IMapper> _mapperMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly CreateBeerCommandHandler _handler;

    /// <summary>
    ///     Setups CreateBeerCommandHandlerTests.
    /// </summary>
    public CreateBeerCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateBeerCommandHandler(_contextMock.Object, _mapperMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method creates beer and returns correct dto.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCreateBeerAndReturnCorrectBeerDto()
    {
        // Arrange
        var request = new CreateBeerCommand
        {
            Name = "Test beer",
            Brewery = "Test brewery",
            AlcoholByVolume = 5.0,
            Description = "Test description",
            Blg = 12.0,
            Plato = 10.0,
            Style = "Test style",
            Ibu = 25,
            Country = "Test country"
        };
        var beerDbSetMock = new List<Beer>().AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Beers).Returns(beerDbSetMock.Object);
        _mapperMock.Setup(m => m.Map<BeerDto>(It.IsAny<Beer>()))
            .Returns((Beer source) => new BeerDto
            {
                Name = source.Name,
                Brewery = source.Brewery,
                AlcoholByVolume = source.AlcoholByVolume,
                Description = source.Description,
                Blg = source.Blg,
                Plato = source.Plato,
                Style = source.Style,
                Ibu = source.Ibu,
                Country = source.Country
            });

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Brewery.Should().Be(request.Brewery);
        result.AlcoholByVolume.Should().Be(request.AlcoholByVolume);
        result.Description.Should().Be(request.Description);
        result.Blg.Should().Be(request.Blg);
        result.Plato.Should().Be(request.Plato);
        result.Style.Should().Be(request.Style);
        result.Ibu.Should().Be(request.Ibu);
        result.Country.Should().Be(request.Country);

        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}