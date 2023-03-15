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
    ///     The query service.
    /// </summary>
    private readonly IQueryService<Beer> _queryService;

    /// <summary>
    ///     The mapper.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    ///     Initializes GetBeersQueryHandler
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="queryService">The query service</param>
    /// <param name="mapper">The mapper</param>
    public GetBeersQueryHandler(IApplicationDbContext context, IQueryService<Beer> queryService, IMapper mapper)
    {
        _context = context;
        _queryService = queryService;
        _mapper = mapper;
    }

    /// <summary>
    ///     Handles GetBeersQuery.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<PaginatedList<BeerDto>> Handle(GetBeersQuery request, CancellationToken cancellationToken)
    {
        var beersCollection = _context.Beers.AsQueryable();

        var delegates = BeersFilteringHelper.GetDelegates(request);
        var sortingColumn = BeersFilteringHelper.GetSortingColumn(request.SortBy);

        beersCollection = _queryService.Filter(beersCollection, delegates);
        beersCollection = _queryService.Sort(beersCollection, sortingColumn, request.SortDirection);
        
        return await beersCollection.ProjectTo<BeerDto>(_mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request.PageNumber, request.PageSize);
    }
}