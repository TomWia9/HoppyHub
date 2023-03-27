﻿using Application.Breweries.Dtos;
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
    ///     Initializes GetBreweriesQueryHandler.
    /// </summary>
    public GetBreweriesQueryHandler(IApplicationDbContext context, IQueryService<Brewery> queryService, IMapper mapper)
    {
        _context = context;
        _queryService = queryService;
        _mapper = mapper;
    }

    /// <summary>
    ///     Handles GetBreweriesQuery.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<PaginatedList<BreweryDto>> Handle(GetBreweriesQuery request, CancellationToken cancellationToken)
    {
        var breweriesCollection = _context.Breweries.AsQueryable();

        var delegates = BreweriesFilteringHelper.GetDelegates(request);
        var sortingColumn = BreweriesFilteringHelper.GetSortingColumn(request.SortBy);

        breweriesCollection = _queryService.Filter(breweriesCollection, delegates);
        breweriesCollection = _queryService.Sort(breweriesCollection, sortingColumn, request.SortDirection);

        return await breweriesCollection.ProjectTo<BreweryDto>(_mapper.ConfigurationProvider)
            .ToPaginatedListAsync(request.PageNumber, request.PageSize);
    }
}