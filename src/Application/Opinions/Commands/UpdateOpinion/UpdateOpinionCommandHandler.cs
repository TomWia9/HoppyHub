using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Entities;
using MediatR;

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
    ///     The beers service.
    /// </summary>
    private readonly IBeersService _beersService;
    
    /// <summary>
    ///     The opinions service.
    /// </summary>
    private readonly IOpinionsService _opinionsService;

    /// <summary>
    ///     Initializes UpdateOpinionCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="currentUserService">The current user service</param>
    /// <param name="beersService">The beers service</param>
    /// <param name="opinionsService">The opinions service</param>
    public UpdateOpinionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService,
        IBeersService beersService, IOpinionsService opinionsService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _beersService = beersService;
        _opinionsService = opinionsService;
    }

    /// <summary>
    ///     Handles UpdateOpinionCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task Handle(UpdateOpinionCommand request, CancellationToken cancellationToken)
    {
        var entity =
            await _context.Opinions.FindAsync(new object?[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Opinion), request.Id);
        }

        if (!_currentUserService.AdministratorAccess &&
            entity.CreatedBy != _currentUserService.UserId)
        {
            throw new ForbiddenAccessException();
        }

        var entityImageUri = entity.ImageUri;

        if (request.Image != null)
        {
            entity.ImageUri = await _opinionsService.HandleOpinionImageUploadAsync(request.Image, entity.BeerId);
        }
        else
        {
            entity.ImageUri = null;
        }

        entity.Rating = request.Rating;
        entity.Comment = request.Comment;

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _beersService.CalculateBeerRatingAsync(entity.BeerId);
            await _context.SaveChangesAsync(cancellationToken);

            if (request.Image == null && !string.IsNullOrEmpty(entityImageUri))
            {
                await _opinionsService.HandleOpinionImageDeleteAsync(entityImageUri);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}