﻿using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Beers.Commands.UpdateBeer;

/// <summary>
///     UpdateBeerCommand handler.
/// </summary>
public class UpdateBeerCommandHandler : IRequestHandler<UpdateBeerCommand>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes the UpdateBeerCommandHandler.
    /// </summary>
    /// <param name="context"></param>
    public UpdateBeerCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    ///     Handles UpdateBeerCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(UpdateBeerCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Breweries.AnyAsync(x => x.Id == request.BreweryId, cancellationToken: cancellationToken))
        {
            throw new NotFoundException(nameof(Brewery), request.BreweryId);
        }

        var entity = await _context.Beers.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Beer), request.Id);
        }

        entity.Name = request.Name;
        entity.BreweryId = request.BreweryId;
        entity.AlcoholByVolume = request.AlcoholByVolume;
        entity.Description = request.Description;
        entity.Blg = request.Blg;
        entity.Plato = request.Plato;
        entity.BeerStyleId = request.BeerStyleId;
        entity.Ibu = request.Ibu;

        await _context.SaveChangesAsync(cancellationToken);
    }
}