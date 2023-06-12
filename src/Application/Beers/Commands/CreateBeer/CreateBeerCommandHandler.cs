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
    ///     The beers service.
    /// </summary>
    private readonly IBeersService _beersService;

    /// <summary>
    ///     Initializes CreateBeerCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="mapper">The mapper</param>
    /// <param name="beersService">The beers service</param>
    public CreateBeerCommandHandler(IApplicationDbContext context, IMapper mapper, IBeersService beersService)
    {
        _context = context;
        _mapper = mapper;
        _beersService = beersService;
    }

    /// <summary>
    ///     Handles CreateBeerCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<BeerDto> Handle(CreateBeerCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Breweries.AnyAsync(x => x.Id == request.BreweryId, cancellationToken: cancellationToken))
        {
            throw new NotFoundException(nameof(Brewery), request.BreweryId);
        }
        
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
            Composition = request.Composition,
            Blg = request.Blg,
            BeerStyleId = request.BeerStyleId,
            Ibu = request.Ibu,
            ReleaseDate = request.ReleaseDate,
            BeerImage = new BeerImage
            {
                TempImage = true,
                ImageUri = _beersService.GetTempImageUri()
            }
        };

        await _context.Beers.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var beerDto = _mapper.Map<BeerDto>(entity);

        return beerDto;
    }
}