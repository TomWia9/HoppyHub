﻿using Application.Breweries.Commands.UpdateBrewery;
using Application.Common.Interfaces;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;
using SharedUtilities.Exceptions;

namespace Application.UnitTests.Breweries.Commands.UpdateBrewery;

/// <summary>
///     Unit tests for the <see cref="UpdateBreweryCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateBreweryCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly UpdateBreweryCommandHandler _handler;

    /// <summary>
    ///     Setups UpdateBreweryCommandHandlerTests.
    /// </summary>
    public UpdateBreweryCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new UpdateBreweryCommandHandler(_contextMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method updates brewery when Brewery exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldUpdateBrewery_WhenBreweryExists()
    {
        // Arrange
        var breweryId = Guid.NewGuid();
        var existingBrewery = new Brewery
        {
            Id = breweryId, Name = "Old Name", Description = "Old description", FoundationYear = 1999,
            Address = new Address { BreweryId = breweryId }, Beers = new List<Beer>()
        };
        var breweries = new List<Brewery> { existingBrewery };
        var breweriesDbSetMock = breweries.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(x => x.Breweries)
            .Returns(breweriesDbSetMock.Object);

        var command = new UpdateBreweryCommand
            { Id = breweryId, Name = "New Name", Description = "New description", FoundationYear = 2010 };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingBrewery.Name.Should().Be(command.Name);
        existingBrewery.Description.Should().Be(command.Description);
        existingBrewery.FoundationYear.Should().Be(command.FoundationYear);
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when Brewery does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBreweryDoesNotExist()
    {
        // Arrange
        var breweries = Enumerable.Empty<Brewery>();
        var breweriesDbSetMock = breweries.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(x => x.Breweries).Returns(breweriesDbSetMock.Object);
        var command = new UpdateBreweryCommand { Id = Guid.NewGuid(), Name = "New Name" };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
    }
}