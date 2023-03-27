using Application.Breweries.Queries.GetBrewery;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Breweries.Queries.GetBrewery;

/// <summary>
///     Tests for the <see cref="GetBreweryQueryHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetBreweryQueryHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly GetBreweryQueryHandler _handler;

    /// <summary>
    ///     Setups GetBreweryQueryHandlerTests.
    /// </summary>
    public GetBreweryQueryHandlerTests()
    {
        var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        var mapper = configurationProvider.CreateMapper();

        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new GetBreweryQueryHandler(_contextMock.Object, mapper);
    }

    /// <summary>
    ///     Tests that Handle method returns BreweryDto when Id is valid.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnBreweryDto_WhenIdIsValid()
    {
        // Arrange
        var breweryId = Guid.NewGuid();
        var brewery = new Brewery { Id = breweryId, Name = "Test brewery" };
        var breweries = new List<Brewery> { brewery };
        var breweriesDbSetMock = breweries.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(x => x.Breweries).Returns(breweriesDbSetMock.Object);

        var query = new GetBreweryQuery() { Id = breweryId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(brewery.Id);
        result.Name.Should().Be(brewery.Name);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when Id is invalid.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenIdIsInvalid()
    {
        // Arrange
        var breweries = Enumerable.Empty<Brewery>();
        var breweriesDbSetMock = breweries.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(x => x.Breweries).Returns(breweriesDbSetMock.Object);
        var query = new GetBreweryQuery { Id = Guid.NewGuid() };

        // Act & Assert
        await _handler.Invoking(x => x.Handle(query, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }
}