using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Opinions.Queries.GetOpinion;
using AutoMapper;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;
using SharedUtilities.Exceptions;

namespace Application.UnitTests.Opinions.Queries.GetOpinion;

/// <summary>
///     Tests for the <see cref="GetOpinionQueryHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetOpinionQueryHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly GetOpinionQueryHandler _handler;

    /// <summary>
    ///     Setups GetBeerQueryHandlerTests.
    /// </summary>
    public GetOpinionQueryHandlerTests()
    {
        var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        var mapper = configurationProvider.CreateMapper();

        _contextMock = new Mock<IApplicationDbContext>();
        _handler = new GetOpinionQueryHandler(_contextMock.Object, mapper);
    }

    /// <summary>
    ///     Tests that Handle method returns OpinionDto with Username when Id is valid and CreatedBy is not null.
    /// </summary>
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handle_ShouldReturnOpinionDtoWithUsername_WhenIdIsValidAndCreatedByIsNotNull(bool userDeleted)
    {
        // Arrange
        const string username = "testUser";
        var opinionId = Guid.NewGuid();
        var opinion = new Opinion
            { Id = opinionId, Rating = 8, CreatedBy = Guid.NewGuid(), User = new User { Username = username, Deleted = userDeleted} };
        var opinions = new List<Opinion> { opinion };
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();
        var query = new GetOpinionQuery { Id = opinionId };

        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(opinion.Id);
        result.Rating.Should().Be(opinion.Rating);
        result.Username.Should().Be(username);
        result.UserDeleted.Should().Be(userDeleted);
    }
    
    /// <summary>
    ///     Tests that Handle method returns OpinionDto without Username when Id is valid and CreatedBy is null.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnOpinionDtoWithoutUsername_WhenIdIsValidAndCreatedByIsNull()
    {
        // Arrange
        var opinionId = Guid.NewGuid();
        var opinion = new Opinion { Id = opinionId, Rating = 8 };
        var opinions = new List<Opinion> { opinion };
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();
        var query = new GetOpinionQuery { Id = opinionId };

        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(opinion.Id);
        result.Rating.Should().Be(opinion.Rating);
        result.Username.Should().BeNull();
    }

    /// <summary>
    ///     Tests that Handle method throws NotFoundException when Id is invalid.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldThrowNotFoundException_WhenIdIsInvalid()
    {
        // Arrange
        var opinions = Enumerable.Empty<Opinion>();
        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        var query = new GetOpinionQuery { Id = Guid.NewGuid() };

        // Act & Assert
        await _handler.Invoking(x => x.Handle(query, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }
}