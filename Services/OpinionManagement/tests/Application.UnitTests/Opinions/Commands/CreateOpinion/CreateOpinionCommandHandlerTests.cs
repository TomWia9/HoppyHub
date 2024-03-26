using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Opinions.Commands.CreateOpinion;
using Application.Opinions.Dtos;
using Application.UnitTests.TestHelpers;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

namespace Application.UnitTests.Opinions.Commands.CreateOpinion;

/// <summary>
///     Unit tests for the <see cref="CreateOpinionCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateOpinionCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The form file mock.
    /// </summary>
    private readonly Mock<IFormFile> _formFileMock;

    /// <summary>
    ///     The opinions service mock.
    /// </summary>
    private readonly Mock<IOpinionsService> _opinionsServiceMock;

    /// <summary>
    ///     The storage container service mock.
    /// </summary>
    private readonly Mock<IStorageContainerService> _storageContainerServiceMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly CreateOpinionCommandHandler _handler;

    /// <summary>
    ///     Setups CreateOpinionCommandHandlerTests.
    /// </summary>
    public CreateOpinionCommandHandlerTests()
    {
        var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        var mapper = configurationProvider.CreateMapper();

        _contextMock = new Mock<IApplicationDbContext>();
        _formFileMock = new Mock<IFormFile>();
        _opinionsServiceMock = new Mock<IOpinionsService>();
        _storageContainerServiceMock = new Mock<IStorageContainerService>();

        _handler = new CreateOpinionCommandHandler(_contextMock.Object, mapper, _opinionsServiceMock.Object,
            _storageContainerServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method creates opinion, uploads image to storage container, publishes BeerOpinionChanged event
    ///     and returns correct dto when opinion contains image.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldCreateOpinionAndUploadImageToStorageContainerAndPublishBeerOpinionChangedEventAndReturnCorrectOpinionDto_WhenOpinionContainsImage()
    {
        // Arrange
        const string username = "testUser";
        const string imageUri = "blob.com/opinions/test.jpg";

        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var request = new CreateOpinionCommand
        {
            Rating = 6,
            Comment = "Sample comment",
            BeerId = beerId,
            Image = _formFileMock.Object
        };
        var expectedOpinionDto = new OpinionDto
        {
            BeerId = request.BeerId,
            Rating = request.Rating,
            Comment = request.Comment,
            ImageUri = imageUri,
            Username = username
        };
        var user = new User
        {
            Username = username
        };
        var beer = new Beer
        {
            Id = beerId, BreweryId = breweryId, Name = "testName", BreweryName = "testBreweryName",
            Opinions = new List<Opinion>()
        };
        var opinions = Enumerable.Empty<Opinion>();
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Beers.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _contextMock.Setup(x => x.Users.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _storageContainerServiceMock.Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<IFormFile>()))
            .ReturnsAsync(imageUri);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OpinionDto>();
        result.Should().BeEquivalentTo(expectedOpinionDto);

        _contextMock.Verify(x => x.Opinions.AddAsync(It.IsAny<Opinion>(), CancellationToken.None), Times.Once);
        _storageContainerServiceMock.Verify(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Once);
        _opinionsServiceMock.Verify(x => x.PublishOpinionChangedEventAsync(beerId, It.IsAny<CancellationToken>()),
            Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Exactly(2));
    }

    /// <summary>
    ///     Tests that Handle method creates opinion, publishes BeerOpinionChanged event
    ///     and returns correct dto when opinion does not contain image.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldCreateOpinionAndPublishBeerOpinionChangedEventAndReturnCorrectOpinionDto_WhenOpinionDoesNotContainImage()
    {
        // Arrange
        const string username = "testUser";
        var beerId = Guid.NewGuid();
        var request = new CreateOpinionCommand
        {
            Rating = 6,
            Comment = "Sample comment",
            BeerId = beerId
        };
        var user = new User
        {
            Username = username
        };
        var expectedOpinionDto = new OpinionDto
        {
            BeerId = beerId,
            Rating = request.Rating,
            Comment = request.Comment,
            ImageUri = null,
            Username = username
        };
        var beer = new Beer { Id = beerId };
        var opinions = Enumerable.Empty<Opinion>();
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Beers.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _contextMock.Setup(x => x.Users.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OpinionDto>();
        result.Should().BeEquivalentTo(expectedOpinionDto);

        _contextMock.Verify(x => x.Opinions.AddAsync(It.IsAny<Opinion>(), CancellationToken.None), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        _storageContainerServiceMock.Verify(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<IFormFile>()), Times.Never);
        _opinionsServiceMock.Verify(x => x.PublishOpinionChangedEventAsync(beerId, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method rollbacks transaction and throws exception when error occurs.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldRollbackTransactionAndThrowException_WhenErrorOccurs()
    {
        // Arrange
        const string exceptionMessage = "Error occurred.";
        var beerId = Guid.NewGuid();
        var request = new CreateOpinionCommand
        {
            Rating = 6,
            Comment = "Sample comment",
            BeerId = beerId,
            Image = _formFileMock.Object
        };
        var beer = new Beer { Id = beerId };
        var opinions = Enumerable.Empty<Opinion>();
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Beers.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _storageContainerServiceMock.Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<IFormFile>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        await _handler.Invoking(x => x.Handle(request, CancellationToken.None))
            .Should().ThrowAsync<Exception>().WithMessage(exceptionMessage);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when beer does not exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundExceptionWhenBeerDoesNotExist()
    {
        // Arrange
        var beerId = Guid.NewGuid();
        var command = new CreateOpinionCommand
        {
            Rating = 6,
            Comment = "Sample comment",
            BeerId = beerId
        };

        _contextMock.Setup(x => x.Beers.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Beer?)null);

        var expectedMessage = $"Entity \"{nameof(Beer)}\" ({beerId}) was not found.";

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>().WithMessage(expectedMessage);
    }

    /// <summary>
    ///     Tests that Handle method throws RemoteServiceConnectionException when failed to upload image.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowRemoteServiceConnectionException_WhenFailedToUploadImage()
    {
        // Arrange
        const string expectedMessage = "Failed to upload image.";
        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var command = new CreateOpinionCommand
        {
            Rating = 6,
            Comment = "Sample comment",
            BeerId = beerId,
            Image = _formFileMock.Object
        };
        var beer = new Beer
        {
            Id = beerId, BreweryId = breweryId, Name = "testName", BreweryName = "testBreweryName",
            Opinions = new List<Opinion>()
        };
        var opinions = Enumerable.Empty<Opinion>();
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Beers.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _storageContainerServiceMock.Setup(x => x.UploadAsync(It.IsAny<string>(), It.IsAny<IFormFile>()))
            .ReturnsAsync(string.Empty);

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<RemoteServiceConnectionException>().WithMessage(expectedMessage);
    }
}