﻿using System.Linq.Expressions;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Opinions.Dtos;
using Application.Opinions.Queries.GetOpinions;
using AutoMapper;
using Domain.Entities;
using MockQueryable.Moq;
using Moq;
using SharedUtilities.Enums;
using SharedUtilities.Interfaces;
using SharedUtilities.Models;

namespace Application.UnitTests.Opinions.Queries.GetOpinions;

/// <summary>
///     Unit tests for the <see cref="GetOpinionsQueryHandler" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetOpinionsQueryHandlerTests
{
    /// <summary>
    ///     The database context mock.
    /// </summary>
    private readonly Mock<IApplicationDbContext> _contextMock;

    /// <summary>
    ///     The handler.
    /// </summary>
    private readonly GetOpinionsQueryHandler _handler;

    /// <summary>
    ///     The QueryService mock.
    /// </summary>
    private readonly Mock<IQueryService<Opinion>> _queryServiceMock;

    /// <summary>
    ///     Setups GetOpinionsQueryHandlerTests.
    /// </summary>
    public GetOpinionsQueryHandlerTests()
    {
        var configurationProvider = new MapperConfiguration(cfg => { cfg.AddProfile<MappingProfile>(); });
        var mapper = configurationProvider.CreateMapper();

        Mock<IFilteringHelper<Opinion, GetOpinionsQuery>> filteringHelperMock = new();
        _contextMock = new Mock<IApplicationDbContext>();
        _queryServiceMock = new Mock<IQueryService<Opinion>>();
        _handler = new GetOpinionsQueryHandler(_contextMock.Object, _queryServiceMock.Object, mapper,
            filteringHelperMock.Object);
    }

    /// <summary>
    ///     Tests that Handle method returns PaginatedList of OpinionDto.
    /// </summary>
    [Fact]
    public async Task Handle_ShouldReturnPaginatedListOfOpinionDto()
    {
        // Arrange
        const string username = "testUser";
        var userId = Guid.NewGuid();
        var request = new GetOpinionsQuery { PageNumber = 1, PageSize = 10 };
        var opinions = new List<Opinion>
        {
            new()
            {
                Id = Guid.NewGuid(), Rating = 4, Comment = "Sample comment", BeerId = Guid.NewGuid(),
                CreatedBy = userId, Created = DateTime.Now, LastModified = DateTime.Now,
                User = new User { Id = userId, Username = username, Deleted = false }
            },
            new()
            {
                Id = Guid.NewGuid(), Rating = 6, Comment = "Sample comment", BeerId = Guid.NewGuid(),
                CreatedBy = userId, Created = DateTime.Now, LastModified = DateTime.Now,
                User = new User { Id = userId, Username = username, Deleted = false }
            },
            new()
            {
                Id = Guid.NewGuid(), Rating = 8, Comment = "Sample comment", BeerId = Guid.NewGuid(),
                CreatedBy = userId, Created = DateTime.Now, LastModified = DateTime.Now,
                User = new User { Id = userId, Username = username, Deleted = true }
            }
        };
        var expectedResult = PaginatedList<OpinionDto>.Create(opinions.Select(x => new OpinionDto
        {
            Id = x.Id,
            Rating = x.Rating,
            Comment = x.Comment,
            BeerId = x.BeerId,
            CreatedBy = x.CreatedBy,
            Created = x.Created,
            LastModified = x.LastModified,
            Username = x.User?.Username,
            UserDeleted = x.User?.Deleted ?? false
        }), 1, 10);

        var opinionsDbSetMock = opinions.AsQueryable().BuildMockDbSet();

        _contextMock.Setup(x => x.Opinions).Returns(opinionsDbSetMock.Object);
        _queryServiceMock.Setup(x =>
                x.Filter(It.IsAny<IQueryable<Opinion>>(), It.IsAny<IEnumerable<Expression<Func<Opinion, bool>>>>()))
            .Returns(opinionsDbSetMock.Object);
        _queryServiceMock.Setup(x =>
                x.Sort(It.IsAny<IQueryable<Opinion>>(), It.IsAny<Expression<Func<Opinion, object>>>(),
                    It.IsAny<SortDirection>()))
            .Returns(opinionsDbSetMock.Object);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<PaginatedList<OpinionDto>>();
        result.Count.Should().Be(3);
        result.Should().BeEquivalentTo(expectedResult);
    }
}