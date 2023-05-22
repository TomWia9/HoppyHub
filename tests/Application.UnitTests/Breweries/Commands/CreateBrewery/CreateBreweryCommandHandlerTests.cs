using Application.Breweries.Commands.CreateBrewery;
using Application.Breweries.Dtos;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Breweries.Commands.CreateBrewery;

/// <summary>
///     Unit tests for the <see cref="CreateBreweryCommandHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateBreweryCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly CreateBreweryCommandHandler _handler;

    /// <summary>
    ///     Setups CreateBreweryCommandHandlerTests.
    /// </summary>
    public CreateBreweryCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        var mapper = configurationProvider.CreateMapper();
        _handler = new CreateBreweryCommandHandler(_contextMock.Object, mapper);
    }

    /// <summary>
    ///     Tests that Handle method creates brewery and returns correct dto.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCreateBreweryAndReturnCorrectBreweryDto()
    {
        // Arrange
        var request = new CreateBreweryCommand
        {
            Name = "Test Brewery",
            Description = "Test Description",
            FoundationYear = 1999,
            WebsiteUrl = "Test WebsiteUrl",
            Street = "Test Street",
            Number = "9A",
            PostCode = "Test PostCode",
            City = "Test City",
            State = "Test State",
            Country = "Test Country"
        };
        var breweries = Enumerable.Empty<Brewery>();
        var breweriesDbSetMock = breweries.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Breweries).Returns(breweriesDbSetMock.Object);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BreweryDto>();
        result.Name.Should().Be(request.Name);
        result.Description.Should().Be(request.Description);
        result.FoundationYear.Should().Be(request.FoundationYear);
        result.WebsiteUrl.Should().Be(request.WebsiteUrl);
        result.Address.Should().NotBeNull();
        result.Address.Should().BeOfType<AddressDto>();
        result.Address!.PostCode.Should().Be(request.PostCode);
        result.Address!.State.Should().Be(request.State);
        result.Address!.Street.Should().Be(request.Street);
        result.Address!.Country.Should().Be(request.Country);
        result.Address!.Number.Should().Be(request.Number);
        result.Address!.City.Should().Be(request.City);

        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}