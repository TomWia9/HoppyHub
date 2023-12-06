using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedEvents.Events;
using SharedEvents.Responses;
using SharedUtilities.Exceptions;
using SharedUtilities.Extensions;

namespace Application.Common.Services;

/// <summary>
///     The opinions service.
/// </summary>
public class OpinionsService : IOpinionsService
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The image created request client.
    /// </summary>
    private readonly IRequestClient<ImageCreated> _imageCreatedRequestClient;

    /// <summary>
    ///     The image deleted request client.
    /// </summary>
    private readonly IRequestClient<ImageDeleted> _imageDeletedRequestClient;

    /// <summary>
    ///     The publish endpoint.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Initializes OpinionsService.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="publishEndpoint">The publish endpoint</param>
    /// <param name="imageCreatedRequestClient">The image created request client</param>
    /// <param name="imageDeletedRequestClient">The image deleted request client</param>
    public OpinionsService(IApplicationDbContext context, IPublishEndpoint publishEndpoint,
        IRequestClient<ImageCreated> imageCreatedRequestClient, IRequestClient<ImageDeleted> imageDeletedRequestClient)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
        _imageCreatedRequestClient = imageCreatedRequestClient;
        _imageDeletedRequestClient = imageDeletedRequestClient;
    }

    /// <summary>
    ///     Uploads image.
    /// </summary>
    /// <param name="entity">The opinion entity</param>
    /// <param name="image">The image</param>
    /// <param name="breweryId">The brewery id</param>
    /// <param name="beerId">The beer id</param>
    /// <param name="opinionId">The opinion id</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task UploadImageAsync(Opinion entity, IFormFile? image, Guid breweryId, Guid beerId, Guid opinionId,
        CancellationToken cancellationToken)
    {
        if (image is not null)
        {
            var imageCreatedEvent = new ImageCreated
            {
                Path = CreateOpinionImagePath(breweryId, beerId, opinionId, image.FileName),
                Image = await image.GetBytes()
            };

            var imageUploadResult =
                await _imageCreatedRequestClient.GetResponse<ImageUploaded>(imageCreatedEvent, cancellationToken);

            var imageUri = imageUploadResult.Message.Uri;

            if (string.IsNullOrEmpty(imageUri))
            {
                throw new RemoteServiceConnectionException("Failed to upload image.");
            }

            entity.ImageUri = imageUri;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /// <summary>
    ///     Deletes the image.
    /// </summary>
    /// <param name="uri">The image uri</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task DeleteImageAsync(string? uri, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(uri))
        {
            var imageDeletedEvent = new ImageDeleted
            {
                Uri = uri
            };

            var imageDeletionResult =
                await _imageDeletedRequestClient.GetResponse<ImageDeletedFromBlobStorage>(imageDeletedEvent,
                    cancellationToken);

            if (!imageDeletionResult.Message.Success)
            {
                throw new RemoteServiceConnectionException("Failed to delete the image.");
            }
        }
    }

    /// <summary>
    ///     Publishes OpinionChanged event
    /// </summary>
    /// <param name="beerId">The beer id</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task PublishOpinionChangedEventAsync(Guid beerId, CancellationToken cancellationToken)
    {
        var beerOpinions = _context.Opinions.Where(x => x.BeerId == beerId);
        var newBeerOpinionsCount = await beerOpinions.CountAsync(cancellationToken: cancellationToken);
        var newBeerRating = !await beerOpinions.AnyAsync(cancellationToken: cancellationToken)
            ? 0
            : await beerOpinions.AverageAsync(x => x.Rating, cancellationToken);
        var beerOpinionChanged = new BeerOpinionChanged
        {
            BeerId = beerId,
            OpinionsCount = newBeerOpinionsCount,
            NewBeerRating = Math.Round(newBeerRating, 2)
        };

        await _publishEndpoint.Publish(beerOpinionChanged, cancellationToken);
    }

    private static string CreateOpinionImagePath(Guid breweryId, Guid beerId, Guid opinionId, string imageName) =>
        $"Opinions/{breweryId.ToString()}/{beerId.ToString()}/{opinionId.ToString()}{Path.GetExtension(imageName)}";
}