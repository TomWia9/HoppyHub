using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Opinions.Commands.CreateOpinion;
using Application.Opinions.Dtos;
using Application.UnitTests.TestHelpers;
using AutoMapper;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Opinions.Commands.CreateOpinion;

/// <summary>
///     Unit tests for the <see cref="CreateOpinionCommandHandler"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class CreateOpinionCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The users service mock.
    /// </summary>
    private readonly Mock<IUsersService> _usersServiceMock;

    /// <summary>
    ///     The mapper mock.
    /// </summary>
    private readonly Mock<IMapper> _mapperMock;

    /// <summary>
    ///     The beers service mock.
    /// </summary>
    private readonly Mock<IBeersService> _beersServiceMock;

    /// <summary>
    ///     The opinions service mock.
    /// </summary>
    private readonly Mock<IOpinionsService> _opinionsServiceMock;

    /// <summary>
    ///     The form file mock.
    /// </summary>
    private readonly Mock<IFormFile> _formFileMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly CreateOpinionCommandHandler _handler;

    /// <summary>
    ///     Setups CreateOpinionCommandHandlerTests.
    /// </summary>
    public CreateOpinionCommandHandlerTests()
    {
        _mapperMock = new Mock<IMapper>();
        _contextMock = new Mock<IApplicationDbContext>();
        _usersServiceMock = new Mock<IUsersService>();
        _beersServiceMock = new Mock<IBeersService>();
        _opinionsServiceMock = new Mock<IOpinionsService>();
        _formFileMock = new Mock<IFormFile>();

        _handler = new CreateOpinionCommandHandler(_contextMock.Object, _mapperMock.Object, _usersServiceMock.Object,
            _beersServiceMock.Object, _opinionsServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method creates opinion, calculates beer rating,
    ///     uploads image to blob storage and returns correct dto when opinion contains image.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldCreateOpinionAndCalculateBeerRatingAndUploadImageAndReturnCorrectOpinionDto_WhenOpinionContainImage()
    {
        // Arrange
        const string username = "testUser";
        const string imageUri = "blob.com/opinions/test.jpg";

        _opinionsServiceMock
            .Setup(x => x.UploadOpinionImageAsync(It.IsAny<IFormFile>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(imageUri);

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
            CreatedBy = Guid.NewGuid()
        };
        var beer = new Beer { Id = beerId, BreweryId = breweryId };
        var opinions = Enumerable.Empty<Opinion>();
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Beers.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _usersServiceMock.Setup(x => x.GetUsernameAsync(It.IsAny<Guid>())).ReturnsAsync(username);
        _mapperMock.Setup(x => x.Map<OpinionDto>(It.IsAny<Opinion>()))
            .Returns(expectedOpinionDto);
        _beersServiceMock.Setup(s => s.CalculateBeerRatingAsync(request.BeerId)).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OpinionDto>();
        result.Rating.Should().Be(request.Rating);
        result.Comment.Should().Be(request.Comment);
        result.BeerId.Should().Be(request.BeerId);
        result.ImageUri.Should().Be(imageUri);
        result.Username.Should().Be(username);

        _contextMock.Verify(x => x.Opinions.AddAsync(It.IsAny<Opinion>(), CancellationToken.None), Times.Once);
        _beersServiceMock.Verify(x => x.CalculateBeerRatingAsync(beerId), Times.Once);
        _opinionsServiceMock.Verify(x => x.UploadOpinionImageAsync(request.Image, breweryId, beerId), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Exactly(2));
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
        var expectedOpinionDto = new OpinionDto
        {
            BeerId = beerId,
            Rating = request.Rating,
            Comment = request.Comment,
            ImageUri = null,
            CreatedBy = Guid.NewGuid()
        };
        var beer = new Beer { Id = beerId };
        var opinions = Enumerable.Empty<Opinion>();
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Beers.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(beer);
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _usersServiceMock.Setup(x => x.GetUsernameAsync(It.IsAny<Guid>())).ReturnsAsync(username);
        _mapperMock.Setup(x => x.Map<OpinionDto>(It.IsAny<Opinion>()))
            .Returns(expectedOpinionDto);
        _beersServiceMock.Setup(s => s.CalculateBeerRatingAsync(request.BeerId)).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OpinionDto>();
        result.Rating.Should().Be(request.Rating);
        result.Comment.Should().Be(request.Comment);
        result.BeerId.Should().Be(request.BeerId);
        result.ImageUri.Should().Be(null);
        result.Username.Should().Be(username);

        _contextMock.Verify(x => x.Opinions.AddAsync(It.IsAny<Opinion>(), CancellationToken.None), Times.Once);
        _beersServiceMock.Verify(x => x.CalculateBeerRatingAsync(beerId), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Exactly(2));
        _opinionsServiceMock.Verify(
            x => x.UploadOpinionImageAsync(It.IsAny<IFormFile>(), It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
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