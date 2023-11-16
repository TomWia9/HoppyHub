using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedEvents.Events;
using SharedEvents.Responses;
using SharedUtilities.Exceptions;

namespace Application.BeerImages.Commands.DeleteBeerImage;

/// <summary>
///     DeleteBeerImageCommand handler.
/// </summary>
public class DeleteBeerImageCommandHandler : IRequestHandler<DeleteBeerImageCommand>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The image deleted request client.
    /// </summary>
    private readonly IRequestClient<ImageDeleted> _imageDeletedRequestClient;

    /// <summary>
    ///     The app configuration.
    /// </summary>
    private readonly IAppConfiguration _appConfiguration;

    /// <summary>
    ///     Initializes DeleteBeerImageCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="imageDeletedRequestClient">The image deleted request client</param>
    /// <param name="appConfiguration">The app configuration</param>
    public DeleteBeerImageCommandHandler(IApplicationDbContext context,
        IRequestClient<ImageDeleted> imageDeletedRequestClient,
        IAppConfiguration appConfiguration)
    {
        _context = context;
        _imageDeletedRequestClient = imageDeletedRequestClient;
        _appConfiguration = appConfiguration;
    }

    /// <summary>
    ///     Handles DeleteBeerImageCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(DeleteBeerImageCommand request, CancellationToken cancellationToken)
    {
        var beer = await _context.Beers.Include(x => x.BeerImage)
            .FirstOrDefaultAsync(x => x.Id == request.BeerId, cancellationToken);

        if (beer is null)
        {
            throw new NotFoundException(nameof(Beer), request.BeerId);
        }

        if (beer.BeerImage is { TempImage: false, ImageUri: not null })
        {
            var beerImageDeleted = new ImageDeleted
            {
                Uri = beer.BeerImage.ImageUri
            };

            var response =
                await _imageDeletedRequestClient.GetResponse<ImageDeletedFromBlobStorage>(beerImageDeleted,
                    cancellationToken);
            var imageDeletedSuccessfully = response.Message.Success;
            if (imageDeletedSuccessfully)
            {
                beer.BeerImage.ImageUri = _appConfiguration.TempBeerImageUri;
                beer.BeerImage.TempImage = true;

                await _context.SaveChangesAsync(CancellationToken.None);
            }
            else
            {
                throw new RemoteServiceConnectionException("There was a problem deleting the image.");
            }
        }
        else
        {
            throw new BadRequestException("Image already deleted.");
        }
    }
}