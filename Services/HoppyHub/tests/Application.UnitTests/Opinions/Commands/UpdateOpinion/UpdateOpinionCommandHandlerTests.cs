using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Opinions.Commands.UpdateOpinion;
using Application.UnitTests.TestHelpers;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Opinions.Commands.UpdateOpinion;

/// <summary>
///     Unit tests for the <see cref="UpdateOpinionCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateOpinionCommandHandlerTests
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
    ///     The opinions images service mock.
    /// </summary>
    private readonly Mock<IOpinionsImagesService> _opinionsImagesServiceMock;

    /// <summary>
    ///     Setups UpdateOpinionCommandHandlerTests.
    /// </summary>
    public UpdateOpinionCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _beersServiceMock = new Mock<IBeersService>();
        _opinionsImagesServiceMock = new Mock<IOpinionsImagesService>();
        _formFileMock = new Mock<IFormFile>();

        _handler = new UpdateOpinionCommandHandler(_contextMock.Object, _currentUserServiceMock.Object,
            _beersServiceMock.Object, _opinionsImagesServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method updates opinion and uploads image when user updates his own opinion
    ///     and opinion exists and updated opinion contains image.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldUpdateOpinionAndUploadImage_WhenUserUpdatesHisOwnOpinionAndOpinionExistsAndUpdatedOpinionContainsImage()
    {
        // Arrange
        const string imagePath = "Opinions/test.jpg";
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

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _opinionsImagesServiceMock.Setup(x => x.CreateImagePath(_formFileMock.Object, breweryId,
            beerId, opinionId)).Returns(imagePath);
        _opinionsImagesServiceMock
            .Setup(x => x.UploadImageAsync(imagePath, _formFileMock.Object))
            .ReturnsAsync(imageUri);

        var command = new UpdateOpinionCommand
        {
            Id = opinionId,
            Rating = 7,
            Comment = "New comment",
            Image = _formFileMock.Object
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Exactly(2));
        _beersServiceMock.Verify(x => x.CalculateBeerRatingAsync(It.IsAny<Guid>()), Times.Once);
        _opinionsImagesServiceMock.Verify(
            x => x.CreateImagePath(_formFileMock.Object, breweryId, beerId, opinionId), Times.Once);
        _opinionsImagesServiceMock.Verify(
            x => x.UploadImageAsync(imagePath, _formFileMock.Object), Times.Once);
        _opinionsImagesServiceMock.Verify(x => x.DeleteImageAsync(It.IsAny<string>()), Times.Never);
    }

    /// <summary>
    ///     Tests that Handle method updates opinion and deletes image when user updates his own opinion
    ///     and opinion exists and opinion contains image and updated opinion does not contains image.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldUpdateOpinionAndDeleteImage_WhenUserUpdatesHisOwnOpinionAndOpinionExistsAndOpinionContainsImageAndUpdatedOpinionDoesNotContainsImage()
    {
        // Arrange
        var opinionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var beerId = Guid.NewGuid();
        var beer = new Beer { Id = beerId, BreweryId = Guid.NewGuid() };
        var existingOpinion = new Opinion
        {
            Id = opinionId, Rating = 9, BeerId = beerId, Beer = beer, Comment = "Sample comment", CreatedBy = userId,
            ImageUri = "test.com"
        };
        var opinions = new List<Opinion> { existingOpinion };
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _opinionsImagesServiceMock
            .Setup(x => x.DeleteImageAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

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
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Exactly(2));
        _beersServiceMock.Verify(x => x.CalculateBeerRatingAsync(It.IsAny<Guid>()), Times.Once);
        _opinionsImagesServiceMock.Verify(x => x.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()),
            Times.Never);
        _opinionsImagesServiceMock.Verify(x => x.DeleteImageAsync(It.IsAny<string>()), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method updates opinion when user updates his own opinion
    ///     and opinion exists and opinion does not contain image and updated opinion also does not contains image.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldUpdateOpinion_WhenUserUpdatesHisOwnOpinionAndOpinionExistsAndOpinionDoesNotContainImageAndUpdatedOpinionAlsoDoesNotContainsImage()
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
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Exactly(2));
        _beersServiceMock.Verify(x => x.CalculateBeerRatingAsync(It.IsAny<Guid>()), Times.Once);
        _opinionsImagesServiceMock.Verify(
            x => x.UploadImageAsync(It.IsAny<string>(), It.IsAny<IFormFile>()),
            Times.Never);
        _opinionsImagesServiceMock.Verify(x => x.DeleteImageAsync(It.IsAny<string>()), Times.Never);
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
    ///     Tests that Handle method updates opinion when user tries to update not his opinion but he has admin access.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldUpdateOpinion_WhenUserTriesToUpdateNotHisOpinionButHasAdminAccess()
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
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Exactly(2));
        _beersServiceMock.Verify(x => x.CalculateBeerRatingAsync(It.IsAny<Guid>()), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method rollbacks transaction and throws exception when error occurs.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldRollbackTransactionAndThrowException_WhenErrorOccurs()
    {
        // Arrange
        const string exceptionMessage = "Error occurred while calculating beer rating";
        var opinionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingOpinion = new Opinion
            { Id = opinionId, Rating = 9, BeerId = Guid.NewGuid(), Comment = "Sample comment", CreatedBy = userId };
        var opinions = new List<Opinion> { existingOpinion };
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();
        var command = new UpdateOpinionCommand
        {
            Id = opinionId,
            Rating = 7,
            Comment = "New comment"
        };
        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _beersServiceMock.Setup(x => x.CalculateBeerRatingAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<Exception>().WithMessage(exceptionMessage);
    }
}