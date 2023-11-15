using Application.BeerImages.Commands.UpsertBeerImage;
using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Moq;
using SharedEvents;
using SharedUtilities.Exceptions;

namespace Application.UnitTests.BeerImages.Commands.UpsertBeerImage;

/// <summary>
///     Unit tests for the <see cref="UpsertBeerImageCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpsertBeerImageCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The publish endpoint mock.
    /// </summary>
    private readonly Mock<IPublishEndpoint> _publishEndpointMock;

    /// <summary>
    ///     The form file mock.
    /// </summary>
    private readonly Mock<IFormFile> _formFileMock;

    /// <summary>
    ///     The UpsertBeerImageCommand handler.
    /// </summary>
    private readonly UpsertBeerImageCommandHandler _handler;

    /// <summary>
    ///     Setups UpsertBeerImageCommandHandlerTests.
    /// </summary>
    public UpsertBeerImageCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _publishEndpointMock = new Mock<IPublishEndpoint>();
        _formFileMock = new Mock<IFormFile>();

        _handler = new UpsertBeerImageCommandHandler(_contextMock.Object, _publishEndpointMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method publishes BeerImageCreated event when beer exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldPublishBeerImageCreatedEvent_WhenBeerExits()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var request = new UpsertBeerImageCommand
        {
            BeerId = beerId,
            Image = _formFileMock.Object
        };
        var beer = new Beer
        {
            Id = beerId,
            BreweryId = breweryId
        };
        var expectedEvent = new ImageCreated
        {
            Path = $"Beers/{beer.BreweryId.ToString()}/{beer.Id.ToString()}",
            Image = request.Image
        };

        _contextMock
            .Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);

        // Act
        await _handler.Handle(request, CancellationToken.None);

        // Assert
        _publishEndpointMock.Verify(x => x.Publish(It.Is<ImageCreated>(y =>
                y.Path == expectedEvent.Path && y.Image == expectedEvent.Image),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when beer does not exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenBeerDoesNotExists()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var command = new UpsertBeerImageCommand
        {
            BeerId = beerId,
            Image = _formFileMock.Object
        };

        _contextMock
            .Setup(x => x.Beers.FindAsync(new object[] { beerId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Beer?)null);

        var expectedMessage = $"Entity \"{nameof(Beer)}\" ({beerId}) was not found.";

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>().WithMessage(expectedMessage);
    }
}