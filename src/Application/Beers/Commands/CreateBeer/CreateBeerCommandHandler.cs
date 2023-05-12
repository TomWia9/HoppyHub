using Application.Beers.Dtos;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Beers.Commands.CreateBeer;

/// <summary>
///     CreateBeerCommand handler.
/// </summary>
public class CreateBeerCommandHandler : IRequestHandler<CreateBeerCommand, BeerDto>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The mapper.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    ///     Initializes CreateBeerCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="mapper">The mapper</param>
    public CreateBeerCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    ///     Handles CreateBeerCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns></returns>
    public async Task<BeerDto> Handle(CreateBeerCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Breweries.AnyAsync(x => x.Id == request.BreweryId, cancellationToken: cancellationToken))
        {
            throw new NotFoundException(nameof(Brewery), request.BreweryId);
        }
        
        //TODO: Add unit test
        if (!await _context.BeerStyles.AnyAsync(x => x.Id == request.BeerStyleId, cancellationToken: cancellationToken))
        {
            throw new NotFoundException(nameof(BeerStyle), request.BeerStyleId);
        }
        
        var entity = new Beer
        {
            Name = request.Name,
            BreweryId = request.BreweryId,
            AlcoholByVolume = request.AlcoholByVolume,
            Description = request.Description,
            Blg = request.Blg,
            Plato = request.Plato,
            BeerStyleId = request.BeerStyleId,
            Ibu = request.Ibu
        };

        await _context.Beers.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var beerDto = _mapper.Map<BeerDto>(entity);

        return beerDto;
    }
}