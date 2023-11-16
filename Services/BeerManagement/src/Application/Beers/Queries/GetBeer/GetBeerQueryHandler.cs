using Application.Beers.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedUtilities.Exceptions;

namespace Application.Beers.Queries.GetBeer;

/// <summary>
///     GetBeerQuery handler.
/// </summary>
public class GetBeerQueryHandler : IRequestHandler<GetBeerQuery, BeerDto>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The mapper
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    ///     Initializes GetBeerQueryHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="mapper">The mapper</param>
    public GetBeerQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    ///     Handles GetBeerQuery.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<BeerDto> Handle(GetBeerQuery request, CancellationToken cancellationToken)
    {
        var beer = await _context.Beers.Include(x => x.Brewery)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (beer is null)
        {
            throw new NotFoundException(nameof(Beer), request.Id);
        }

        return _mapper.Map<BeerDto>(beer);
    }
}