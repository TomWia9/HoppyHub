using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedEvents.Events;
using SharedEvents.Responses;
using SharedUtilities.Exceptions;
using SharedUtilities.Extensions;
using SharedUtilities.Interfaces;

namespace Application.Opinions.Commands.UpdateOpinion;

/// <summary>
///     UpdateOpinionCommand handler.
/// </summary>
public class UpdateOpinionCommandHandler : IRequestHandler<UpdateOpinionCommand>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The current user service.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

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
    ///     Initializes UpdateOpinionCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="currentUserService">The current user service</param>
    /// <param name="imageCreatedRequestClient">The image created request client</param>
    /// <param name="publishEndpoint">The publish endpoint</param>
    /// <param name="imageDeletedRequestClient">The image deleted request client</param>
    public UpdateOpinionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService,
        IRequestClient<ImageCreated> imageCreatedRequestClient, IPublishEndpoint publishEndpoint,
        IRequestClient<ImageDeleted> imageDeletedRequestClient)
    {
        _context = context;
        _currentUserService = currentUserService;
        _imageCreatedRequestClient = imageCreatedRequestClient;
        _publishEndpoint = publishEndpoint;
        _imageDeletedRequestClient = imageDeletedRequestClient;
    }

    /// <summary>
    ///     Handles UpdateOpinionCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(UpdateOpinionCommand request, CancellationToken cancellationToken)
    {
        var entity =
            await _context.Opinions.Include(x => x.Beer)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity is null)
        {
            throw new NotFoundException(nameof(Opinion), request.Id);
        }

        if (!_currentUserService.AdministratorAccess &&
            entity.CreatedBy != _currentUserService.UserId)
        {
            throw new ForbiddenAccessException();
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            if (request.Image is null && !string.IsNullOrEmpty(entity.ImageUri))
            {
                await DeleteImageAsync(entity.ImageUri, cancellationToken);
            }

            if (request.Image is not null)
            {
                await UploadImageAsync(entity, request.Image, entity.Beer!.BreweryId, entity.BeerId, entity.Id,
                    cancellationToken);
            }
            else
            {
                entity.ImageUri = null;
            }

            entity.Rating = request.Rating;
            entity.Comment = request.Comment;
            await _context.SaveChangesAsync(cancellationToken);

            await SendOpinionChangedEventAsync(entity.BeerId, cancellationToken);
            
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    //TODO: Move to separate service.
    /// <summary>
    ///     Uploads image.
    /// </summary>
    /// <param name="entity">The opinion entity</param>
    /// <param name="image">The image</param>
    /// <param name="breweryId">The brewery id</param>
    /// <param name="beerId">The beer id</param>
    /// <param name="opinionId">The opinion id</param>
    /// <param name="cancellationToken">The cancellation token</param>
    private async Task UploadImageAsync(Opinion entity, IFormFile? image, Guid breweryId, Guid beerId, Guid opinionId,
        CancellationToken cancellationToken)
    {
        if (image is not null)
        {
            var imageCreatedEvent = new ImageCreated
            {
                //TODO: Move to GetImagePath method in new service
                Path =
                    $"Opinions/{breweryId.ToString()}/{beerId.ToString()}/{opinionId.ToString()}{Path.GetExtension(image.FileName)}",
                Image = await image.GetBytes()
            };

            var imageUploadResult =
                await _imageCreatedRequestClient.GetResponse<ImageUploaded>(imageCreatedEvent, cancellationToken);

            var imageUri = imageUploadResult.Message.Uri;

            if (string.IsNullOrEmpty(imageUri))
            {
                throw new RemoteServiceConnectionException("Failed to sent image.");
            }

            entity.ImageUri = imageUri;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    //TODO: Move to separate service.
    /// <summary>
    ///     Deletes the image.
    /// </summary>
    /// <param name="uri">The image uri</param>
    /// <param name="cancellationToken">The cancellation token</param>
    private async Task DeleteImageAsync(string? uri, CancellationToken cancellationToken)
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

    //TODO: Move to separate service.
    /// <summary>
    ///     Sends OpinionChanged event
    /// </summary>
    /// <param name="beerId">The beer id</param>
    /// <param name="cancellationToken">The cancellation token</param>
    private async Task SendOpinionChangedEventAsync(Guid beerId, CancellationToken cancellationToken)
    {
        var newBeerOpinionsCount =
            await _context.Opinions.CountAsync(x => x.BeerId == beerId,
                cancellationToken: cancellationToken);
        var newBeerRating = await _context.Opinions.Where(x => x.BeerId == beerId)
            .AverageAsync(x => x.Rating, cancellationToken: cancellationToken);
        var opinionChanged = new OpinionChanged
        {
            BeerId = beerId,
            OpinionsCount = newBeerOpinionsCount,
            NewBeerRating = Math.Round(newBeerRating, 2)
        };

        await _publishEndpoint.Publish(opinionChanged, cancellationToken);
    }
}