using Application.BeerStyles.Commands.CreateBeerStyle;
using Application.BeerStyles.Dtos;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;
using SharedUtilities.Mappings;

namespace Application.UnitTests.BeerStyles.Commands.CreateBeerStyle;

/// <summary>
///     Unit tests for the <see cref="CreateBeerStyleCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateBeerStyleCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly CreateBeerStyleCommandHandler _handler;

    /// <summary>
    ///     Setups CreateBeerStyleCommandHandlerTests.
    /// </summary>
    public CreateBeerStyleCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        var mapper = configurationProvider.CreateMapper();
        _handler = new CreateBeerStyleCommandHandler(_contextMock.Object, mapper);
    }

    /// <summary>
    ///     Tests that Handle method creates beer style and returns correct dto.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCreateBeerStyleAndReturnCorrectBeerStyleDto()
    {
        // Arrange
        var request = new CreateBeerStyleCommand
        {
            Name = "India Pale Ale",
            Description = "Test Description",
            CountryOfOrigin = "England"
        };
        var beerStyles = Enumerable.Empty<BeerStyle>();
        var beerStylesDbSetMock = beerStyles.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.BeerStyles).Returns(beerStylesDbSetMock.Object);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BeerStyleDto>();
        result.Name.Should().Be(request.Name);
        result.Description.Should().Be(request.Description);
        result.CountryOfOrigin.Should().Be(request.CountryOfOrigin);

        _contextMock.Verify(x => x.BeerStyles.AddAsync(It.IsAny<BeerStyle>(), CancellationToken.None), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}