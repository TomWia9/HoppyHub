using Application.Beers.Dtos;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedEvents.Events;
using SharedUtilities.Exceptions;

namespace Application.Beers.Commands.CreateBeer;

/// <summary>
///     CreateBeerCommand handler.
/// </summary>
public class CreateBeerCommandHandler : IRequestHandler<CreateBeerCommand, BeerDto>
{
    /// <summary>
    ///     The app configuration.
    /// </summary>
    private readonly IAppConfiguration _appConfiguration;

    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The mapper.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    ///     The publish endpoint.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Initializes CreateBeerCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="mapper">The mapper</param>
    /// <param name="publishEndpoint">The publish endpoint</param>
    /// <param name="appConfiguration">The app configuration</param>
    public CreateBeerCommandHandler(IApplicationDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint,
        IAppConfiguration appConfiguration)
    {
        _context = context;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
        _appConfiguration = appConfiguration;
    }

    /// <summary>
    ///     Handles CreateBeerCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<BeerDto> Handle(CreateBeerCommand request, CancellationToken cancellationToken)
    {
        var beerBrewery = await _context.Breweries.FindAsync([request.BreweryId],
            cancellationToken);

        if (beerBrewery is null)
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
                ImageUri = _appConfiguration.TempBeerImageUri
            }
        };

        await _context.Beers.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var beerCreatedEvent = new BeerCreated
        {
            Id = entity.Id,
            Name = entity.Name,
            BreweryId = entity.BreweryId
        };

        await _publishEndpoint.Publish(beerCreatedEvent, cancellationToken);

        var beerDto = _mapper.Map<BeerDto>(entity);

        return beerDto;
    }
}