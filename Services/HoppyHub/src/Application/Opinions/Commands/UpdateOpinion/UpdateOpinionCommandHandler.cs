﻿using Application.Common.Interfaces;
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
    ///     The beers service.
    /// </summary>
    private readonly IBeersService _beersService;

    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The current user service.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    ///     The opinions images service.
    /// </summary>
    private readonly IOpinionsImagesService _opinionsImagesService;

    /// <summary>
    ///     Initializes UpdateOpinionCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="currentUserService">The current user service</param>
    /// <param name="beersService">The beers service</param>
    /// <param name="opinionsImagesService">The opinions images service</param>
    public UpdateOpinionCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService,
        IBeersService beersService, IOpinionsImagesService opinionsImagesService)
    {
        _context = context;
        _currentUserService = currentUserService;
        _beersService = beersService;
        _opinionsImagesService = opinionsImagesService;
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

        var entityImageUri = entity.ImageUri;

        if (request.Image is not null)
        {
            var imagePath =
                _opinionsImagesService.CreateImagePath(request.Image, entity.Beer!.BreweryId, entity.BeerId, entity.Id);
            entity.ImageUri =
                await _opinionsImagesService.UploadImageAsync(imagePath, request.Image);
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

            if (request.Image is null && !string.IsNullOrEmpty(entityImageUri))
            {
                await _opinionsImagesService.DeleteImageAsync(entityImageUri);
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