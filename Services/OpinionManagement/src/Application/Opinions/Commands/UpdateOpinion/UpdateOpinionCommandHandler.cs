using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SharedUtilities.Exceptions;
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
    ///     The opinions service.
    /// </summary>
    private readonly IOpinionsService _opinionsService;

    /// <summary>
    ///     The storage container service.
    /// </summary>
    private readonly IStorageContainerService _storageContainerService;

    /// <summary>
    ///     Initializes UpdateOpinionCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="currentUserService">The current user service</param>
    /// <param name="opinionsService">TThe opinions service</param>
    /// <param name="storageContainerService">The storage container service</param>
    public UpdateOpinionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService,
        IOpinionsService opinionsService, IStorageContainerService storageContainerService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _opinionsService = opinionsService;
        _storageContainerService = storageContainerService;
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
                var opinionImagePath = $"Opinions/{entity.Beer!.BreweryId}/{entity.BeerId}/{entity.Id}";

                await _storageContainerService.DeleteFromPathAsync(opinionImagePath);
            }

            if (request.Image is not null)
            {
                var fileName =
                    $"Opinions/{entity.Beer!.BreweryId.ToString()}/{entity.Beer.Id.ToString()}/{entity.Id.ToString()}{Path.GetExtension(request.Image.FileName)}";

                var imageUri = await _storageContainerService.UploadAsync(fileName, request.Image);

                if (string.IsNullOrEmpty(imageUri))
                {
                    throw new RemoteServiceConnectionException("Failed to upload image.");
                }

                entity.ImageUri = imageUri;
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                entity.ImageUri = null;
            }

            entity.Rating = request.Rating;
            entity.Comment = request.Comment;
            await _context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            await _opinionsService.PublishOpinionChangedEventAsync(entity.BeerId, cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}