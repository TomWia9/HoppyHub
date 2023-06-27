﻿using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Opinions.Dtos;
using AutoMapper;
using Domain.Entities;
using MediatR;

namespace Application.Opinions.Commands.CreateOpinion;

/// <summary>
///     CreateOpinionCommand handler.
/// </summary>
public class CreateOpinionCommandHandler : IRequestHandler<CreateOpinionCommand, OpinionDto>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The mapper.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    ///     The users service.
    /// </summary>
    private readonly IUsersService _usersService;

    /// <summary>
    ///     The beers service.
    /// </summary>
    private readonly IBeersService _beersService;

    /// <summary>
    ///     The images service.
    /// </summary>
    private readonly IImagesService<Opinion> _imagesService;

    /// <summary>
    ///     Initializes CreateOpinionCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="mapper">The mapper</param>
    /// <param name="usersService">The users service</param>
    /// <param name="beersService">The beers service</param>
    /// <param name="imagesService">The images service</param>
    public CreateOpinionCommandHandler(IApplicationDbContext context, IMapper mapper, IUsersService usersService,
        IBeersService beersService, IImagesService<Opinion> imagesService)
    {
        _context = context;
        _mapper = mapper;
        _usersService = usersService;
        _beersService = beersService;
        _imagesService = imagesService;
    }

    /// <summary>
    ///     Handles CreateOpinionCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<OpinionDto> Handle(CreateOpinionCommand request, CancellationToken cancellationToken)
    {
        var beer = await _context.Beers.FindAsync(new object?[] { request.BeerId },
            cancellationToken: cancellationToken);

        if (beer is null)
        {
            throw new NotFoundException(nameof(Beer), request.BeerId);
        }

        var entity = new Opinion
        {
            Rating = request.Rating,
            Comment = request.Comment,
            BeerId = request.BeerId
        };

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _context.Opinions.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            if (request.Image is not null)
            {
                entity.ImageUri = await _imagesService.UploadImageAsync(request.Image, beer.BreweryId,
                    request.BeerId, entity.Id);

                await _context.SaveChangesAsync(cancellationToken);
            }

            await _beersService.CalculateBeerRatingAsync(request.BeerId);
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        var opinionDto = _mapper.Map<OpinionDto>(entity);
        opinionDto.Username = opinionDto.CreatedBy is null
            ? null
            : await _usersService.GetUsernameAsync(opinionDto.CreatedBy.Value);

        return opinionDto;
    }
}