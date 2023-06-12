using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.BeerImages.Commands.DeleteBeerImage;

/// <summary>
///     DeleteBeerImageCommand handler
/// </summary>
public class DeleteBeerImageCommandHandler : IRequestHandler<DeleteBeerImageCommand>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The beers service.
    /// </summary>
    private readonly IBeersService _beersService;

    /// <summary>
    ///     Initializes DeleteBeerImageCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="beersService">The beers service</param>
    public DeleteBeerImageCommandHandler(IApplicationDbContext context, IBeersService beersService)
    {
        _context = context;
        _beersService = beersService;
    }

    /// <summary>
    ///     Handles DeleteBeerImageCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(DeleteBeerImageCommand request, CancellationToken cancellationToken)
    {
        var beer = await _context.Beers.Include(x => x.BeerImage)
            .FirstOrDefaultAsync(x => x.Id == request.BeerId, cancellationToken: cancellationToken);

        if (beer == null)
        {
            throw new NotFoundException(nameof(Beer), request.BeerId);
        }

        if (beer.BeerImage is { TempImage: false, ImageUri: not null })
        {
            var beerImageUri = beer.BeerImage.ImageUri;

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                beer.BeerImage.ImageUri = _beersService.GetTempImageUri();
                beer.BeerImage.TempImage = true;
                await _context.SaveChangesAsync(cancellationToken);

                await _beersService.DeleteBeerImageAsync(beerImageUri);

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}