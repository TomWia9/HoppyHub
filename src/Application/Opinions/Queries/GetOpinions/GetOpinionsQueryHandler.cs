using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using Application.Opinions.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;

namespace Application.Opinions.Queries.GetOpinions;

/// <summary>
///     GetOpinionsQuery handler.
/// </summary>
public class GetOpinionsQueryHandler : IRequestHandler<GetOpinionsQuery, PaginatedList<OpinionDto>>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The query service.
    /// </summary>
    private readonly IQueryService<Opinion> _queryService;

    /// <summary>
    ///     The mapper.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    ///     The users service.
    /// </summary>
    private readonly IUsersService _usersService;

    /// <summary>
    ///     Initializes GetOpinionsQueryHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="mapper">The mapper</param>
    /// <param name="queryService">The query service</param>
    /// <param name="usersService">The users service</param>
    public GetOpinionsQueryHandler(IApplicationDbContext context, IQueryService<Opinion> queryService, IMapper mapper,
        IUsersService usersService)
    {
        _context = context;
        _queryService = queryService;
        _mapper = mapper;
        _usersService = usersService;
    }

    /// <summary>
    ///     Handles GetOpinionsQuery.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<PaginatedList<OpinionDto>> Handle(GetOpinionsQuery request, CancellationToken cancellationToken)
    {
        var opinionsCollection = _context.Opinions.AsQueryable();

        var delegates = OpinionsFilteringHelper.GetDelegates(request);
        var sortingColumn = OpinionsFilteringHelper.GetSortingColumn(request.SortBy);

        opinionsCollection = _queryService.Filter(opinionsCollection, delegates);
        opinionsCollection = _queryService.Sort(opinionsCollection, sortingColumn, request.SortDirection);

        var opinions = await opinionsCollection.ProjectTo<OpinionDto>(_mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request.PageNumber, request.PageSize);

        var users = await _usersService.GetUsersAsync();

        foreach (var opinion in opinions.Where(opinion => opinion.CreatedBy.HasValue))
        {
            if (users.TryGetValue(opinion.CreatedBy!.Value, out var username))
            {
                opinion.Username = username;
            }
        }

        return opinions;
    }
}