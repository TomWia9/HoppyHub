using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Opinions.Commands.CreateOpinion;
using Application.Opinions.Dtos;
using AutoMapper;
using Domain.Entities;
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
        _handler = new CreateOpinionCommandHandler(_contextMock.Object, _mapperMock.Object, _usersServiceMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method creates opinion and returns correct dto.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldCreateOpinionAndReturnCorrectOpinionDto()
    {
        // Arrange
        const string username = "testUser";
        var beerId = Guid.NewGuid();
        var request = new CreateOpinionCommand
        {
            Rate = 6,
            Comment = "Sample comment",
            BeerId = beerId
        };
        var expectedOpinionDto = new OpinionDto
        {
            BeerId = beerId,
            Rate = request.Rate,
            Comment = request.Comment,
            CreatedBy = Guid.NewGuid()
        };
        var beers = new List<Beer> { new() { Id = beerId } };
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();
        var opinions = Enumerable.Empty<Opinion>();
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _usersServiceMock.Setup(x => x.GetUsernameAsync(It.IsAny<Guid>())).ReturnsAsync(username);
        _mapperMock.Setup(x => x.Map<OpinionDto>(It.IsAny<Opinion>()))
            .Returns(expectedOpinionDto);
        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OpinionDto>();
        result.Rate.Should().Be(request.Rate);
        result.Comment.Should().Be(request.Comment);
        result.BeerId.Should().Be(request.BeerId);
        result.Username.Should().Be(username);

        _contextMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
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
            Rate = 6,
            Comment = "Sample comment",
            BeerId = beerId
        };

        var beers = Enumerable.Empty<Beer>();
        var beersDbSetMock = beers.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Beers).Returns(beersDbSetMock.Object);

        var expectedMessage = $"Entity \"{nameof(Beer)}\" ({beerId}) was not found.";

        // Act & Assert
        await _handler.Invoking(x => x.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>().WithMessage(expectedMessage);
    }
}