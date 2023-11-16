using Application.Common.Extensions;
using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedEvents.Events;
using SharedEvents.Responds;
using SharedUtilities.Exceptions;

namespace Application.BeerImages.Commands.UpsertBeerImage;

/// <summary>
///     UpsertBeerImageCommand handler.
/// </summary>
public class UpsertBeerImageCommandHandler : IRequestHandler<UpsertBeerImageCommand, string>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;
    
    /// <summary>
    ///     The image created request client.
    /// </summary>
    readonly IRequestClient<ImageCreated> _imageCreatedRequestClient;

    /// <summary>
    ///     Initializes UpsertBeerImageCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="imageCreatedRequestClient">The image created request client</param>
    public UpsertBeerImageCommandHandler(IApplicationDbContext context,
        IRequestClient<ImageCreated> imageCreatedRequestClient)
    {
        _context = context;
        _imageCreatedRequestClient = imageCreatedRequestClient;
    }

    /// <summary>
    ///     Handles UpsertBeerImageCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<string> Handle(UpsertBeerImageCommand request, CancellationToken cancellationToken)
    {
        var beer = await _context.Beers.FindAsync(new object?[] { request.BeerId },
            cancellationToken);

        if (beer is null)
        {
            throw new NotFoundException(nameof(Beer), request.BeerId);
        }

        var imageCreatedEvent = new ImageCreated
        {
            Path =
                $"Beers/{beer.BreweryId.ToString()}/{beer.Id.ToString()}{Path.GetExtension(request.Image!.FileName)}",
            Image = await request.Image!.GetBytes()
        };
        
        var imageUploadResult =
            await _imageCreatedRequestClient.GetResponse<ImageUploaded>(imageCreatedEvent, cancellationToken);
        var imageUri = imageUploadResult.Message.Uri;
        
        if (string.IsNullOrEmpty(imageUri))
        {
            throw new RemoteServiceConnectionException("test");
        }
        
        var entity = await _context.BeerImages.FirstOrDefaultAsync(x => x.BeerId == request.BeerId,
            cancellationToken: cancellationToken);
        
        if (entity is null)
        {
            entity = new BeerImage
            {
                BeerId = beer.Id,
                ImageUri = imageUri,
                TempImage = false
            };
        
            await _context.BeerImages.AddAsync(entity, cancellationToken);
        }
        else
        {
            entity.ImageUri = imageUri;
            entity.TempImage = false;
        }
        
        await _context.SaveChangesAsync(CancellationToken.None);
        
        return imageUri;
    }
}