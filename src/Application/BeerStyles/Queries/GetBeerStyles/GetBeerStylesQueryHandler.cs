using Application.BeerStyles.Dtos;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;

namespace Application.BeerStyles.Queries.GetBeerStyles;

/// <summary>
///     GetBeerStylesQuery handler.
/// </summary>
public class GetBeerStylesQueryHandler : IRequestHandler<GetBeerStylesQuery, PaginatedList<BeerStyleDto>>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The query service.
    /// </summary>
    private readonly IQueryService<BeerStyle> _queryService;

    /// <summary>
    ///     The mapper.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    ///     The beer styles filtering helper.
    /// </summary>
    private readonly IFilteringHelper<BeerStyle, GetBeerStylesQuery> _filteringHelper;

    /// <summary>
    ///     Initializes GetBeerStylesQueryHandler
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="queryService">The query service</param>
    /// <param name="mapper">The mapper</param>
    /// <param name="filteringHelper">The beer styles filtering helper</param>
    public GetBeerStylesQueryHandler(IApplicationDbContext context, IQueryService<BeerStyle> queryService,
        IMapper mapper, IFilteringHelper<BeerStyle, GetBeerStylesQuery> filteringHelper)
    {
        _context = context;
        _queryService = queryService;
        _mapper = mapper;
        _filteringHelper = filteringHelper;
    }

    /// <summary>
    ///     Handles GetBeerStylesQuery.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<PaginatedList<BeerStyleDto>> Handle(GetBeerStylesQuery request,
        CancellationToken cancellationToken)
    {
        var beerStylesCollection = _context.BeerStyles.AsQueryable();

        var delegates = _filteringHelper.GetDelegates(request);
        var sortingColumn = _filteringHelper.GetSortingColumn(request.SortBy);

        beerStylesCollection = _queryService.Filter(beerStylesCollection, delegates);
        beerStylesCollection = _queryService.Sort(beerStylesCollection, sortingColumn, request.SortDirection);

        return await beerStylesCollection.ProjectTo<BeerStyleDto>(_mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request.PageNumber, request.PageSize);
    }
}