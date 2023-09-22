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
using SharedUtilities.Mappings;

namespace Application.UnitTests.Opinions.Commands.CreateOpinion;

/// <summary>
///     Unit tests for the <see cref="CreateOpinionCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateOpinionCommandHandlerTests
{
    /// <summary>
    ///     The beers service mock.
    /// </summary>
    private readonly Mock<IBeersService> _beersServiceMock;

    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The form file mock.
    /// </summary>
    private readonly Mock<IFormFile> _formFileMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly CreateOpinionCommandHandler _handler;

    /// <summary>
    ///     The opinions images service mock.
    /// </summary>
    private readonly Mock<IOpinionsImagesService> _opinionsImagesServiceMock;

    /// <summary>
    ///     Setups CreateOpinionCommandHandlerTests.
    /// </summary>
    public CreateOpinionCommandHandlerTests()
    {
        var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        var mapper = configurationProvider.CreateMapper();

        _contextMock = new Mock<IApplicationDbContext>();
        _beersServiceMock = new Mock<IBeersService>();
        _opinionsImagesServiceMock = new Mock<IOpinionsImagesService>();
        _formFileMock = new Mock<IFormFile>();

        _handler = new CreateOpinionCommandHandler(_contextMock.Object, mapper, _beersServiceMock.Object,
            _opinionsImagesServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method creates opinion, calculates beer rating,
    ///     uploads image to blob storage and returns correct dto when opinion contains image.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldCreateOpinionAndCalculateBeerRatingAndUploadImageAndReturnCorrectOpinionDto_WhenOpinionContainsImage()
    {
        // Arrange
        const string username = "testUser";
        const string imagePath = "Opinions/test.jpg";
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
            BeerId = beerId,
            Rating = request.Rating,
            Comment = request.Comment,
            ImageUri = imageUri,
            Username = username
        };
        var user = new User
        {
            Username = username
        };
        var beer = new Beer { Id = beerId, BreweryId = breweryId };
        var opinions = Enumerable.Empty<Opinion>();
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _opinionsImagesServiceMock.Setup(x => x.CreateImagePath(It.IsAny<IFormFile>(), It.IsAny<Guid>(),
            It.IsAny<Guid>(),
            It.IsAny<Guid>())).Returns(imagePath);
        _opinionsImagesServiceMock
            .Setup(x => x.UploadImageAsync(imagePath, _formFileMock.Object))
            .ReturnsAsync(imageUri);
        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Beers.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _contextMock.Setup(x => x.Users.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _beersServiceMock.Setup(s => s.CalculateBeerRatingAsync(request.BeerId)).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OpinionDto>();
        result.Should().BeEquivalentTo(expectedOpinionDto);

        _contextMock.Verify(x => x.Opinions.AddAsync(It.IsAny<Opinion>(), CancellationToken.None), Times.Once);
        _beersServiceMock.Verify(x => x.CalculateBeerRatingAsync(beerId), Times.Once);
        _opinionsImagesServiceMock.Verify(x => x.UploadImageAsync(imagePath, request.Image),
            Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Exactly(3));
    }

    /// <summary>
    ///     Tests that Handle method creates opinion, calculates beer rating,
    ///     and returns correct dto when opinion contains image.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldCreateOpinionAndCalculateBeerRatingAndReturnCorrectOpinionDto_WhenOpinionDoesNotContainImage()
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
        _beersServiceMock.Setup(s => s.CalculateBeerRatingAsync(request.BeerId)).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OpinionDto>();
        result.Should().BeEquivalentTo(expectedOpinionDto);

        _contextMock.Verify(x => x.Opinions.AddAsync(It.IsAny<Opinion>(), CancellationToken.None), Times.Once);
        _beersServiceMock.Verify(x => x.CalculateBeerRatingAsync(beerId), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Exactly(2));
        _opinionsImagesServiceMock.Verify(
            x => x.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()),
            Times.Never);
    }

    /// <summary>
    ///     Tests that Handle method rollbacks transaction and throws exception when error occurs.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldRollbackTransactionAndThrowException_WhenErrorOccurs()
    {
        // Arrange
        const string exceptionMessage = "Error occurred while calculating beer rating";
        var beerId = Guid.NewGuid();
        var request = new CreateOpinionCommand
        {
            Rating = 6,
            Comment = "Sample comment",
            BeerId = beerId
        };
        var beer = new Beer { Id = beerId };
        var opinions = Enumerable.Empty<Opinion>();
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Beers.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _beersServiceMock.Setup(x => x.CalculateBeerRatingAsync(request.BeerId))
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
}