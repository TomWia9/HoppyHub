using Application.Common.Interfaces;
using Application.Opinions.Dtos;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using SharedUtilities.Interfaces;
using SharedUtilities.Mappings;
using SharedUtilities.Models;

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
    ///     The opinions filtering helper.
    /// </summary>
    private readonly IFilteringHelper<Opinion, GetOpinionsQuery> _filteringHelper;

    /// <summary>
    ///     The mapper.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    ///     The query service.
    /// </summary>
    private readonly IQueryService<Opinion> _queryService;

    /// <summary>
    ///     Initializes GetOpinionsQueryHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="mapper">The mapper</param>
    /// <param name="queryService">The query service</param>
    /// <param name="filteringHelper">The opinions filtering helper</param>
    public GetOpinionsQueryHandler(IApplicationDbContext context, IQueryService<Opinion> queryService, IMapper mapper,
        IFilteringHelper<Opinion, GetOpinionsQuery> filteringHelper)
    {
        _context = context;
        _queryService = queryService;
        _mapper = mapper;
        _filteringHelper = filteringHelper;
    }

    /// <summary>
    ///     Handles GetOpinionsQuery.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<PaginatedList<OpinionDto>> Handle(GetOpinionsQuery request, CancellationToken cancellationToken)
    {
        var opinionsCollection = _context.Opinions.AsQueryable();

        var delegates = _filteringHelper.GetDelegates(request);
        var sortingColumn = _filteringHelper.GetSortingColumn(request.SortBy);

        opinionsCollection = _queryService.Filter(opinionsCollection, delegates);
        opinionsCollection = _queryService.Sort(opinionsCollection, sortingColumn, request.SortDirection);

        var opinions = await opinionsCollection.ProjectTo<OpinionDto>(_mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request.PageNumber, request.PageSize);

        return opinions;
    }
}