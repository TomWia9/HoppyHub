using Application.Beers.Dtos;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;

namespace Application.Beers.Queries.GetBeers;

/// <summary>
///     GetBeersQuery handler.
/// </summary>
public class GetBeersQueryHandler : IRequestHandler<GetBeersQuery, PaginatedList<BeerDto>>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The beers filtering helper.
    /// </summary>
    private readonly IFilteringHelper<Beer, GetBeersQuery> _filteringHelper;

    /// <summary>
    ///     The mapper.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    ///     The query service.
    /// </summary>
    private readonly IQueryService<Beer> _queryService;

    /// <summary>
    ///     Initializes GetBeersQueryHandler
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="queryService">The query service</param>
    /// <param name="mapper">The mapper</param>
    /// <param name="filteringHelper">The beers filtering helper</param>
    public GetBeersQueryHandler(IApplicationDbContext context, IQueryService<Beer> queryService, IMapper mapper,
        IFilteringHelper<Beer, GetBeersQuery> filteringHelper)
    {
        _context = context;
        _queryService = queryService;
        _mapper = mapper;
        _filteringHelper = filteringHelper;
    }

    /// <summary>
    ///     Handles GetBeersQuery.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<PaginatedList<BeerDto>> Handle(GetBeersQuery request, CancellationToken cancellationToken)
    {
        var beersCollection = _context.Beers.AsQueryable();

        var delegates = _filteringHelper.GetDelegates(request);
        var sortingColumn = _filteringHelper.GetSortingColumn(request.SortBy);

        beersCollection = _queryService.Filter(beersCollection, delegates);
        beersCollection = _queryService.Sort(beersCollection, sortingColumn, request.SortDirection);

        return await beersCollection.ProjectTo<BeerDto>(_mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request.PageNumber, request.PageSize);
    }
}