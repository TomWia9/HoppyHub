using Application.Breweries.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Breweries.Queries.GetBrewery;

/// <summary>
///     GetBreweryQuery handler.
/// </summary>
public class GetBreweryQueryHandler : IRequestHandler<GetBreweryQuery, BreweryDto>
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
    ///     Initializes GetBreweryQueryHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="mapper">The mapper</param>
    public GetBreweryQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    ///     Handles GetBreweryQuery.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<BreweryDto> Handle(GetBreweryQuery request, CancellationToken cancellationToken)
    {
        var brewery = await _context.Breweries.Include(x => x.Address)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (brewery is null)
        {
            throw new NotFoundException(nameof(Brewery), request.Id);
        }

        return _mapper.Map<BreweryDto>(brewery);
    }
}