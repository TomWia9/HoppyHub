﻿using Application.Beers.Dtos;
using Application.Common.Interfaces;
using Application.Common.Mappings;
using Application.Common.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;

namespace Application.Favorites.Queries.GetFavorites;

/// <summary>
///     GetFavoritesQuery handler.
/// </summary>
public class GetFavoritesQueryHandler : IRequestHandler<GetFavoritesQuery, PaginatedList<BeerDto>>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The query service.
    /// </summary>
    private readonly IQueryService<Favorite> _queryService;

    /// <summary>
    ///     The mapper.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    ///     Initializes GetFavoritesQueryHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="queryService">The query service</param>
    /// <param name="mapper">The mapper</param>
    public GetFavoritesQueryHandler(IApplicationDbContext context, IQueryService<Favorite> queryService, IMapper mapper)
    {
        _context = context;
        _queryService = queryService;
        _mapper = mapper;
    }

    /// <summary>
    ///     Handles GetFavoritesQuery.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<PaginatedList<BeerDto>> Handle(GetFavoritesQuery request, CancellationToken cancellationToken)
    {
        var favoritesCollection = _context.Favorites.AsQueryable();

        var delegates = FavoritesFilteringHelper.GetDelegates(request);
        var sortingColumn = FavoritesFilteringHelper.GetSortingColumn(request.SortBy);

        favoritesCollection = _queryService.Filter(favoritesCollection, delegates);
        favoritesCollection = _queryService.Sort(favoritesCollection, sortingColumn, request.SortDirection);

        return await favoritesCollection.Select(x => x.Beer).ProjectTo<BeerDto>(_mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request.PageNumber, request.PageSize);
    }
}