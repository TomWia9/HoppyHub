﻿using Application.Beers.Queries.GetBeer;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.Beers.Queries.GetBeer;

/// <summary>
///     Tests for the <see cref="GetBeerQueryHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetBeerQueryHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly GetBeerQueryHandler _handler;

    /// <summary>
    ///     Setups GetBeerQueryHandlerTests.
    /// </summary>
    public GetBeerQueryHandlerTests()
    {
        var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        var mapper = configurationProvider.CreateMapper();

        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new GetBeerQueryHandler(_contextMock.Object, mapper);
    }

    /// <summary>
    ///     Tests that Handle method returns BeerDto when Id is valid.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnBeerDto_WhenIdIsValid()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var beer = new Beer { Id = beerId, Name = "Test Beer" };
        _contextMock.Setup(c => c.Beers.FindAsync(new object?[] { beerId }, CancellationToken.None))
            .ReturnsAsync(beer);
        var query = new GetBeerQuery { Id = beerId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(beer.Id);
        result.Name.Should().Be(beer.Name);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when Id is invalid.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenIdIsInvalid()
    {
        // Arrange
        _contextMock.Setup(c => c.Beers.FindAsync(new object?[] { Guid.NewGuid() }, CancellationToken.None))
            .ReturnsAsync((Beer?)null);
        var query = new GetBeerQuery { Id = Guid.NewGuid() };

        // Act & Assert
        await _handler.Invoking(h => h.Handle(query, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }
}