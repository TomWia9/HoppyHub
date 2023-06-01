using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Opinions.Commands.DeleteOpinion;
using Application.UnitTests.TestHelpers;
using Domain.Entities;
using Moq;

namespace Application.UnitTests.Opinions.Commands.DeleteOpinion;

/// <summary>
///     Unit tests for the <see cref="DeleteOpinionCommandHandler"/> class.
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
    ///     The beers service mock.
    /// </summary>
    private readonly Mock<IBeersService> _beersServiceMock;

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
        _beersServiceMock = new Mock<IBeersService>();
        _handler = new DeleteOpinionCommandHandler(_contextMock.Object, _currentUserServiceMock.Object,
            _beersServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method removes opinion from database when opinion exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldRemoveOpinionFromDatabaseAndCalculateBeerRating_WhenOpinionExists()
    {
        // Arrange
        var opinionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var opinion = new Opinion { Id = opinionId, CreatedBy = userId };

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Opinions.FindAsync(new object[] { opinionId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(opinion);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _beersServiceMock.Setup(x => x.CalculateBeerRatingAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);
        var command = new DeleteOpinionCommand { Id = opinionId };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.Opinions.Remove(opinion), Times.Once);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        _beersServiceMock.Verify(x => x.CalculateBeerRatingAsync(It.IsAny<Guid>()), Times.Once);
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when opinion does not exist.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenOpinionDoesNotExist()
    {
        // Arrange
        var opinionId = Guid.NewGuid();
        _contextMock.Setup(x => x.Opinions.FindAsync(new object[] { It.IsAny<Guid>() }, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Opinion?)null);
        var command = new DeleteOpinionCommand { Id = opinionId };
        var expectedMessage = $"Entity \"{nameof(Opinion)}\" ({opinionId}) was not found.";

        // Act
        var action = new Func<Task>(() => _handler.Handle(command, CancellationToken.None));

        // Assert
        await action.Should().ThrowAsync<NotFoundException>().WithMessage(expectedMessage);
        _contextMock.Verify(x => x.Opinions.Remove(It.IsAny<Opinion>()), Times.Never);
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    /// <summary>
    ///     Tests that Handle method throws ForbiddenException when user tries to delete not his opinion and user has no admin access.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowForbiddenException_WhenUserTriesToDeleteNotHisOpinionAndUserHasNoAdminAccess()
    {
        // Arrange
        var opinionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingOpinion = new Opinion
            { Id = opinionId, Rating = 9, BeerId = Guid.NewGuid(), Comment = "Sample comment", CreatedBy = userId };

        _contextMock.Setup(x => x.Opinions.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOpinion);
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
    ///      Tests that Handle method removes opinion when user tries to delete not his opinion but he has admin access.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldRemoveOpinionAndCalculateBeerRating_WhenUserTriesToDeleteNotHisOpinionButHasAdminAccess()
    {
        // Arrange
        var opinionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingOpinion = new Opinion
            { Id = opinionId, Rating = 9, BeerId = Guid.NewGuid(), Comment = "Sample comment", CreatedBy = userId };

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Opinions.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOpinion);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(true);

        var command = new DeleteOpinionCommand
        {
            Id = opinionId
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
        var opinion = new Opinion { Id = opinionId, CreatedBy = userId };

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Opinions.FindAsync(new object[] { opinionId }, It.IsAny<CancellationToken>()))
            .ReturnsAsync(opinion);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _beersServiceMock.Setup(x => x.CalculateBeerRatingAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception(exceptionMessage));
        var command = new DeleteOpinionCommand { Id = opinionId };

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<Exception>().WithMessage(exceptionMessage);
    }
}