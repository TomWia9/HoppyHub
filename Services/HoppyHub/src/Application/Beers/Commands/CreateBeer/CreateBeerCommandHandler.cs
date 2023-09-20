using Application.Beers.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedUtilities.Exceptions;

namespace Application.Beers.Commands.CreateBeer;

/// <summary>
///     CreateBeerCommand handler.
/// </summary>
public class CreateBeerCommandHandler : IRequestHandler<CreateBeerCommand, BeerDto>
{
    /// <summary>
    ///     The beer images service.
    /// </summary>
    private readonly IBeersImagesService _beerImagesService;

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
    /// <param name="beerImagesService">The beer images service</param>
    public CreateBeerCommandHandler(IApplicationDbContext context, IMapper mapper,
        IBeersImagesService beerImagesService)
    {
        _context = context;
        _mapper = mapper;
        _beerImagesService = beerImagesService;
    }

    /// <summary>
    ///     Handles CreateBeerCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<BeerDto> Handle(CreateBeerCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Breweries.AnyAsync(x => x.Id == request.BreweryId, cancellationToken))
        {
            throw new NotFoundException(nameof(Brewery), request.BreweryId);
        }

        if (!await _context.BeerStyles.AnyAsync(x => x.Id == request.BeerStyleId, cancellationToken))
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
                ImageUri = _beerImagesService.GetTempBeerImageUri()
            }
        };

        await _context.Beers.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var beerDto = _mapper.Map<BeerDto>(entity);

        return beerDto;
    }
}