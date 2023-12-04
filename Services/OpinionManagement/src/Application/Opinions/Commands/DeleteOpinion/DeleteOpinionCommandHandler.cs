using Application.Common.Interfaces;
using Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedEvents.Events;
using SharedEvents.Responses;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

namespace Application.Opinions.Commands.DeleteOpinion;

/// <summary>
///     DeleteOpinionCommand handler
/// </summary>
public class DeleteOpinionCommandHandler : IRequestHandler<DeleteOpinionCommand>
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
    ///     The image deleted request client.
    /// </summary>
    private readonly IRequestClient<ImageDeleted> _imageDeletedRequestClient;

    /// <summary>
    ///     The publish endpoint.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Initializes DeleteOpinionCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="currentUserService">The current user service</param>
    /// <param name="imageDeletedRequestClient">The image deleted request client</param>
    /// <param name="publishEndpoint">The publish endpoint</param>
    public DeleteOpinionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService,
        IRequestClient<ImageDeleted> imageDeletedRequestClient, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _currentUserService = currentUserService;
        _imageDeletedRequestClient = imageDeletedRequestClient;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    ///     Handles DeleteOpinionCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(DeleteOpinionCommand request, CancellationToken cancellationToken)
    {
        var entity =
            await _context.Opinions.FindAsync(new object?[] { request.Id }, cancellationToken);

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
            _context.Opinions.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            
            await DeleteImageAsync(entity.ImageUri, cancellationToken);
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