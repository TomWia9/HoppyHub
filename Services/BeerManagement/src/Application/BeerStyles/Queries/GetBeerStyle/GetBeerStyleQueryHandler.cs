using Application.BeerStyles.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using SharedUtilities.Exceptions;

namespace Application.BeerStyles.Queries.GetBeerStyle;

/// <summary>
///     GetBeerStyleQuery handler.
/// </summary>
public class GetBeerStyleQueryHandler : IRequestHandler<GetBeerStyleQuery, BeerStyleDto>
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
    ///     Initializes GetBeerStyleQueryHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="mapper">The mapper</param>
    public GetBeerStyleQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    ///     Handles GetBeerStyleQuery.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<BeerStyleDto> Handle(GetBeerStyleQuery request, CancellationToken cancellationToken)
    {
        var beerStyle =
            await _context.BeerStyles.FindAsync([request.Id], cancellationToken);

        if (beerStyle is null)
        {
            throw new NotFoundException(nameof(BeerStyle), request.Id);
        }

        return _mapper.Map<BeerStyleDto>(beerStyle);
    }
}