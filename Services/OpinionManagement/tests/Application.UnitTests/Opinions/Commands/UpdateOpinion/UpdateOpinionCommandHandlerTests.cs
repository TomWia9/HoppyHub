using Application.Common.Interfaces;
using Application.Opinions.Commands.UpdateOpinion;
using Application.UnitTests.TestHelpers;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

namespace Application.UnitTests.Opinions.Commands.UpdateOpinion;

/// <summary>
///     Unit tests for the <see cref="UpdateOpinionCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateOpinionCommandHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The current user service mock.
    /// </summary>
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    /// <summary>
    ///     The form file mock.
    /// </summary>
    private readonly Mock<IFormFile> _formFileMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly UpdateOpinionCommandHandler _handler;

    /// <summary>
    ///     The opinions service mock.
    /// </summary>
    private readonly Mock<IOpinionsService> _opinionsServiceMock;

    /// <summary>
    ///     Setups UpdateOpinionCommandHandlerTests.
    /// </summary>
    public UpdateOpinionCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _opinionsServiceMock = new Mock<IOpinionsService>();
        _formFileMock = new Mock<IFormFile>();

        _handler = new UpdateOpinionCommandHandler(_contextMock.Object, _currentUserServiceMock.Object,
            _opinionsServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method updates opinion, uploads image and publishes BeerOpinionChanged event
    ///     when user updates his own opinion and opinion exists and updated opinion contains image.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldUpdateOpinionAndUploadImageAndPublishBeerOpinionChangedEvent_WhenUserUpdatesHisOwnOpinionAndOpinionExistsAndUpdatedOpinionContainsImage()
    {
        // Arrange
        const string imageUri = "blob.com/opinions/test.jpg";

        var opinionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var beer = new Beer { Id = beerId, BreweryId = breweryId };
        var existingOpinion = new Opinion
        {
            Id = opinionId, Rating = 9, BeerId = beerId, Beer = beer, Comment = "Sample comment", CreatedBy = userId,
            ImageUri = null
        };
        var opinions = new List<Opinion> { existingOpinion };
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();
        var command = new UpdateOpinionCommand
        {
            Id = opinionId,
            Rating = 7,
            Comment = "New comment",
            Image = _formFileMock.Object
        };

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _opinionsServiceMock.Setup(x => x.UploadImageAsync(It.IsAny<Opinion>(), command.Image, breweryId, beerId,
            command.Id,
            It.IsAny<CancellationToken>())).Callback<Opinion, IFormFile?, Guid, Guid, Guid, CancellationToken>(
            (entity, _, _, _, _, _) => { entity.ImageUri = imageUri; });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingOpinion.Comment.Should().Be(command.Comment);
        existingOpinion.Rating.Should().Be(command.Rating);
        existingOpinion.ImageUri.Should().Be(imageUri);
        _opinionsServiceMock.Verify(
            x => x.UploadImageAsync(It.IsAny<Opinion>(), command.Image, breweryId, beerId, command.Id,
                It.IsAny<CancellationToken>()), Times.Once);
        _opinionsServiceMock.Verify(x => x.PublishOpinionChangedEventAsync(beerId, It.IsAny<CancellationToken>()),
            Times.Once);
        _opinionsServiceMock.Verify(x => x.DeleteImageAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method updates opinion, deletes image and publishes BeerOpinionChanged event
    ///     when user updates his own opinion and opinion exists and opinion contains image
    ///     and updated opinion does not contains image.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldUpdateOpinionAndDeleteImageAndPublishBeerOpinionChangedEvent_WhenUserUpdatesHisOwnOpinionAndOpinionExistsAndOpinionContainsImageAndUpdatedOpinionDoesNotContainsImage()
    {
        // Arrange
        const string imageUri = "test.com";
        var opinionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var beerId = Guid.NewGuid();
        var beer = new Beer { Id = beerId, BreweryId = Guid.NewGuid() };
        var existingOpinion = new Opinion
        {
            Id = opinionId, Rating = 9, BeerId = beerId, Beer = beer, Comment = "Sample comment", CreatedBy = userId,
            ImageUri = imageUri
        };
        var opinions = new List<Opinion> { existingOpinion };
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        var command = new UpdateOpinionCommand
        {
            Id = opinionId,
            Rating = 7,
            Comment = "New comment",
            Image = null
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingOpinion.Comment.Should().Be(command.Comment);
        existingOpinion.Rating.Should().Be(command.Rating);
        existingOpinion.ImageUri.Should().Be(null);
        _opinionsServiceMock.Verify(x => x.DeleteImageAsync(imageUri, It.IsAny<CancellationToken>()),
            Times.Once);
        _opinionsServiceMock.Verify(
            x => x.UploadImageAsync(It.IsAny<Opinion>(), It.IsAny<IFormFile?>(), It.IsAny<Guid>(), It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()), Times.Never());
        _opinionsServiceMock.Verify(x => x.PublishOpinionChangedEventAsync(beerId, It.IsAny<CancellationToken>()),
            Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method updates opinion and publishes BeerOpinionChanged event when user updates his own opinion
    ///     and opinion exists and opinion does not contain image and updated opinion also does not contains image.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldUpdateOpinionAndPublishBeerOpinionChangedEvent_WhenUserUpdatesHisOwnOpinionAndOpinionExistsAndOpinionDoesNotContainImageAndUpdatedOpinionAlsoDoesNotContainsImage()
    {
        // Arrange
        var opinionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var beerId = Guid.NewGuid();
        var beer = new Beer { Id = beerId, BreweryId = Guid.NewGuid() };
        var existingOpinion = new Opinion
        {
            Id = opinionId, Rating = 9, BeerId = beerId, Beer = beer, Comment = "Sample comment", CreatedBy = userId,
            ImageUri = null
        };
        var opinions = new List<Opinion> { existingOpinion };
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        var command = new UpdateOpinionCommand
        {
            Id = opinionId,
            Rating = 7,
            Comment = "New comment",
            Image = null
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingOpinion.Comment.Should().Be(command.Comment);
        existingOpinion.Rating.Should().Be(command.Rating);
        _opinionsServiceMock.Verify(x => x.DeleteImageAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _opinionsServiceMock.Verify(
            x => x.UploadImageAsync(It.IsAny<Opinion>(), It.IsAny<IFormFile?>(), It.IsAny<Guid>(), It.IsAny<Guid>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()), Times.Never());
        _opinionsServiceMock.Verify(x => x.PublishOpinionChangedEventAsync(beerId, It.IsAny<CancellationToken>()),
            Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when opinion does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenOpinionDoesNotExist()
    {
        // Arrange
        var opinionId = Guid.NewGuid();
        var opinions = Enumerable.Empty<Opinion>();
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);

        var command = new UpdateOpinionCommand
            { Id = opinionId, Rating = 5, Comment = "Sample comment" };

        var expectedMessage = $"Entity \"{nameof(Opinion)}\" ({opinionId}) was not found.";

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>().WithMessage(expectedMessage);
    }

    /// <summary>
    ///     Tests that Handle method throws ForbiddenException when user tries to update not his opinion and user has no admin
    ///     access.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowForbiddenException_WhenUserTriesToUpdateNotHisOpinionAndUserHasNoAdminAccess()
    {
        // Arrange
        var opinionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingOpinion = new Opinion
            { Id = opinionId, Rating = 9, BeerId = Guid.NewGuid(), Comment = "Sample comment", CreatedBy = userId };
        var opinions = new List<Opinion> { existingOpinion };
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());

        var command = new UpdateOpinionCommand
        {
            Id = opinionId,
            Rating = 7,
            Comment = "New comment"
        };

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<ForbiddenAccessException>();
    }

    /// <summary>
    ///     Tests that Handle method updates opinion and publishes BeerOpinionChanged event
    ///     when user tries to update not his opinion but he has admin access.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldUpdateOpinionAndPublishBeerOpinionChangedEvent_WhenUserTriesToUpdateNotHisOpinionButHasAdminAccess()
    {
        // Arrange
        var opinionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingOpinion = new Opinion
            { Id = opinionId, Rating = 9, BeerId = Guid.NewGuid(), Comment = "Sample comment", CreatedBy = userId };
        var opinions = new List<Opinion> { existingOpinion };
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(true);

        var command = new UpdateOpinionCommand
        {
            Id = opinionId,
            Rating = 7,
            Comment = "New comment"
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        existingOpinion.Comment.Should().Be(command.Comment);
        existingOpinion.Rating.Should().Be(command.Rating);
        _opinionsServiceMock.Verify(
            x => x.PublishOpinionChangedEventAsync(existingOpinion.BeerId, It.IsAny<CancellationToken>()),
            Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
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
        var breweryId = Guid.NewGuid();
        var opinionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var beer = new Beer { Id = beerId, BreweryId = breweryId };
        var existingOpinion = new Opinion
        {
            Id = opinionId, Rating = 9, BeerId = beerId, Beer = beer, Comment = "Sample comment", CreatedBy = userId,
            ImageUri = null
        };
        var opinions = new List<Opinion> { existingOpinion };
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();
        var command = new UpdateOpinionCommand
        {
            Id = opinionId,
            Rating = 7,
            Comment = "New comment",
            Image = _formFileMock.Object
        };
        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _opinionsServiceMock.Setup(x => x.UploadImageAsync(It.IsAny<Opinion>(), It.IsAny<IFormFile?>(),
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<Exception>().WithMessage(exceptionMessage);
    }
}