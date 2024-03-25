using Application.Common.Interfaces;
using Application.Opinions.Commands.DeleteOpinion;
using Application.UnitTests.TestHelpers;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

namespace Application.UnitTests.Opinions.Commands.DeleteOpinion;

/// <summary>
///     Unit tests for the <see cref="DeleteOpinionCommandHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class DeleteOpinionCommandHandlerTests
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
    private readonly DeleteOpinionCommandHandler _handler;

    /// <summary>
    ///     Setups DeleteOpinionCommandHandlerTests.
    /// </summary>
    public DeleteOpinionCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _opinionsServiceMock = new Mock<IOpinionsService>();
        _storageContainerServiceMock = new Mock<IStorageContainerService>();

        _handler = new DeleteOpinionCommandHandler(_contextMock.Object, _currentUserServiceMock.Object,
            _opinionsServiceMock.Object, _storageContainerServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method removes opinion from database, deletes image and
    ///     publishes BeerOpinionChanged event when opinion exists and opinion has image.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldRemoveOpinionFromDatabaseAndAndDeleteImageAndPublishBeerOpinionChangedEvent_WhenOpinionExistsAndOpinionHasImage()
    {
        // Arrange
        var opinionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var beer = new Beer
        {
            Id = beerId,
            BreweryId = breweryId
        };
        var existingOpinion = new Opinion
        {
            Id = opinionId, BeerId = beerId, Beer = beer, CreatedBy = userId, ImageUri = "test.com",
            Created = new DateTimeOffset(),
            LastModified = new DateTimeOffset(), LastModifiedBy = userId
        };
        var command = new DeleteOpinionCommand { Id = opinionId };
        var opinions = new List<Opinion> { existingOpinion };
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.Opinions.Remove(existingOpinion), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _storageContainerServiceMock.Verify(x => x.DeleteFromPathAsync(It.IsAny<string>()), Times.Once);
        _opinionsServiceMock.Verify(x => x.PublishOpinionChangedEventAsync(beerId, It.IsAny<CancellationToken>()),
            Times.Once);
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
        var command = new DeleteOpinionCommand { Id = opinionId };
        var expectedMessage = $"Entity \"{nameof(Opinion)}\" ({opinionId}) was not found.";

        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);

        // Act
        var action = new Func<Task>(() => _handler.Handle(command, CancellationToken.None));

        // Assert
        await action.Should().ThrowAsync<NotFoundException>().WithMessage(expectedMessage);
        _contextMock.Verify(x => x.Opinions.Remove(It.IsAny<Opinion>()), Times.Never);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        _storageContainerServiceMock.Verify(x => x.DeleteFromPathAsync(It.IsAny<string>()), Times.Never);
        _opinionsServiceMock.Verify(
            x => x.PublishOpinionChangedEventAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    /// <summary>
    ///     Tests that Handle method throws ForbiddenException when user tries to delete not his opinion and user has no admin
    ///     access.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowForbiddenException_WhenUserTriesToDeleteNotHisOpinionAndUserHasNoAdminAccess()
    {
        // Arrange
        var opinionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var beer = new Beer
        {
            Id = beerId,
            BreweryId = breweryId
        };
        var existingOpinion = new Opinion
        {
            Id = opinionId, Rating = 9, BeerId = beerId, Beer = beer, Comment = "Sample comment", CreatedBy = userId
        };
        var opinions = new List<Opinion> { existingOpinion };
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());

        var command = new DeleteOpinionCommand
        {
            Id = opinionId
        };

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<ForbiddenAccessException>();
    }

    /// <summary>
    ///     Tests that Handle method removes opinion and image and publishes BeerOpinionChangedEvent when user
    ///     tries to delete not his opinion but he has admin access.
    /// </summary>
    [Fact]
    public async Task
        Handle_ShouldRemoveOpinionAndImageAndPublishBeerOpinionChangedEvent_WhenUserTriesToDeleteNotHisOpinionButHasAdminAccess()
    {
        // Arrange
        var opinionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var beer = new Beer
        {
            Id = beerId,
            BreweryId = breweryId
        };
        var existingOpinion = new Opinion
        {
            Id = opinionId, Rating = 9, BeerId = beerId, Beer = beer, Comment = "Sample comment", CreatedBy = userId,
            ImageUri = "test.com"
        };
        var opinions = new List<Opinion> { existingOpinion };
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(true);

        var command = new DeleteOpinionCommand
        {
            Id = opinionId
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Exactly(1));
        _storageContainerServiceMock.Verify(x => x.DeleteFromPathAsync(It.IsAny<string>()), Times.Once);
        _opinionsServiceMock.Verify(
            x => x.PublishOpinionChangedEventAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
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
        var opinionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var beerId = Guid.NewGuid();
        var breweryId = Guid.NewGuid();
        var beer = new Beer
        {
            Id = beerId,
            BreweryId = breweryId
        };
        var existingOpinion = new Opinion
            { Id = opinionId, BeerId = beerId, Beer = beer, CreatedBy = userId, ImageUri = "test.com" };
        var opinions = new List<Opinion> { existingOpinion };
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();
        var command = new DeleteOpinionCommand { Id = opinionId };

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _storageContainerServiceMock.Setup(x => x.DeleteFromPathAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<Exception>().WithMessage(exceptionMessage);
    }
}