﻿using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Opinions.Commands.DeleteOpinion;
using Application.Opinions.Commands.UpdateOpinion;
using Application.UnitTests.TestHelpers;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;

namespace Application.UnitTests.Opinions.Commands.UpdateOpinion;

/// <summary>
///     Unit tests for the <see cref="UpdateOpinionCommandHandler"/> class.
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
    ///     The beers service mock.
    /// </summary>
    private readonly Mock<IBeersService> _beersServiceMock;
    
    /// <summary>
    ///     The opinions service mock.
    /// </summary>
    private readonly Mock<IOpinionsService> _opinionsServiceMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly UpdateOpinionCommandHandler _handler;

    /// <summary>
    ///     Setups UpdateOpinionCommandHandlerTests.
    /// </summary>
    public UpdateOpinionCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _beersServiceMock = new Mock<IBeersService>();
        _opinionsServiceMock = new Mock<IOpinionsService>();
        
        _handler = new UpdateOpinionCommandHandler(_contextMock.Object, _currentUserServiceMock.Object,
            _beersServiceMock.Object, _opinionsServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method updates opinion when user updates his own opinion and opinion exists.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldUpdateOpinion_WhenUserUpdatesHisOwnOpinionAndOpinionExists()
    {
        // Arrange
        var opinionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingOpinion = new Opinion
            { Id = opinionId, Rating = 9, BeerId = Guid.NewGuid(), Comment = "Sample comment", CreatedBy = userId };

        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Opinions.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOpinion);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);

        var command = new UpdateOpinionCommand
        {
            Id = opinionId,
            Rating = 7,
            Comment = "New comment",
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Exactly(2));
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

        _contextMock.Setup(x => x.Opinions.FindAsync(new object[] { It.IsAny<Guid>() }, CancellationToken.None))
            .ReturnsAsync((Opinion?)null);

        var command = new UpdateOpinionCommand
            { Id = opinionId, Rating = 5, Comment = "Sample comment" };

        var expectedMessage = $"Entity \"{nameof(Opinion)}\" ({opinionId}) was not found.";

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>().WithMessage(expectedMessage);
    }

    /// <summary>
    ///     Tests that Handle method throws ForbiddenException when user tries to update not his opinion and user has no admin access.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowForbiddenException_WhenUserTriesToUpdateNotHisOpinionAndUserHasNoAdminAccess()
    {
        // Arrange
        var opinionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingOpinion = new Opinion
            { Id = opinionId, Rating = 9, BeerId = Guid.NewGuid(), Comment = "Sample comment", CreatedBy = userId };

        _contextMock.Setup(x => x.Opinions.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOpinion);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());

        var command = new UpdateOpinionCommand
        {
            Id = opinionId,
            Rating = 7,
            Comment = "New comment",
        };

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<ForbiddenAccessException>();
    }

    /// <summary>
    ///      Tests that Handle method updates opinion when user tries to update not his opinion but he has admin access.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldUpdateOpinion_WhenUserTriesToUpdateNotHisOpinionButHasAdminAccess()
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

        var command = new UpdateOpinionCommand
        {
            Id = opinionId,
            Rating = 7,
            Comment = "New comment",
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
        var command = new UpdateOpinionCommand
        {
            Id = opinionId,
            Rating = 7,
            Comment = "New comment",
        };
        
        _contextMock.SetupGet(x => x.Database).Returns(new MockDatabaseFacade(_contextMock.Object));
        _contextMock.Setup(x => x.Opinions.FindAsync(It.IsAny<object?[]?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingOpinion);
        _currentUserServiceMock.Setup(x => x.UserId).Returns(userId);
        _beersServiceMock.Setup(x => x.CalculateBeerRatingAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<Exception>().WithMessage(exceptionMessage);
    }
}