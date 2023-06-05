using Application.Breweries.Dtos;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;

namespace Application.Breweries.Queries.GetBreweries;

/// <summary>
///     GetBreweriesQuery handler.
/// </summary>
public class GetBreweriesQueryHandler : IRequestHandler<GetBreweriesQuery, PaginatedList<BreweryDto>>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The query service.
    /// </summary>
    private readonly IQueryService<Brewery> _queryService;

    /// <summary>
    ///     The mapper.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    ///     The breweries filtering helper.
    /// </summary>
    private readonly IFilteringHelper<Brewery, GetBreweriesQuery> _filteringHelper;

    /// <summary>
    ///     Initializes GetBreweriesQueryHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="queryService">The query service</param>
    /// <param name="mapper">The mapper</param>
    /// <param name="filteringHelper">The breweries filtering helper</param>
    public GetBreweriesQueryHandler(IApplicationDbContext context, IQueryService<Brewery> queryService, IMapper mapper,
        IFilteringHelper<Brewery, GetBreweriesQuery> filteringHelper)
    {
        _context = context;
        _queryService = queryService;
        _mapper = mapper;
        _filteringHelper = filteringHelper;
    }

    /// <summary>
    ///     Handles GetBreweriesQuery.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<PaginatedList<BreweryDto>> Handle(GetBreweriesQuery request, CancellationToken cancellationToken)
    {
        var breweriesCollection = _context.Breweries.AsQueryable();

        var delegates = _filteringHelper.GetDelegates(request);
        var sortingColumn = _filteringHelper.GetSortingColumn(request.SortBy);

        breweriesCollection = _queryService.Filter(breweriesCollection, delegates);
        breweriesCollection = _queryService.Sort(breweriesCollection, sortingColumn, request.SortDirection);

        return await breweriesCollection.ProjectTo<BreweryDto>(_mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request.PageNumber, request.PageSize);
    }
}