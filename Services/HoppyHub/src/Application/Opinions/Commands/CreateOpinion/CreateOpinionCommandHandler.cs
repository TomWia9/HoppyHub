using Application.Common.Exceptions;
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
    ///     The beers service.
    /// </summary>
    private readonly IBeersService _beersService;

    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The mapper.
    /// </summary>
    private readonly IMapper _mapper;

    /// <summary>
    ///     The opinions images service.
    /// </summary>
    private readonly IOpinionsImagesService _opinionsImagesService;

    /// <summary>
    ///     Initializes CreateOpinionCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="mapper">The mapper</param>
    /// <param name="beersService">The beers service</param>
    /// <param name="opinionsImagesService">The opinions images service</param>
    public CreateOpinionCommandHandler(IApplicationDbContext context, IMapper mapper, IBeersService beersService,
        IOpinionsImagesService opinionsImagesService)
    {
        _context = context;
        _mapper = mapper;
        _beersService = beersService;
        _opinionsImagesService = opinionsImagesService;
    }

    /// <summary>
    ///     Handles CreateOpinionCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<OpinionDto> Handle(CreateOpinionCommand request, CancellationToken cancellationToken)
    {
        var beer = await _context.Beers.FindAsync(new object?[] { request.BeerId },
            cancellationToken);

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
                var imagePath =
                    _opinionsImagesService.CreateImagePath(request.Image!, beer.BreweryId, beer.Id, entity.Id);

                entity.ImageUri = await _opinionsImagesService.UploadImageAsync(imagePath, request.Image!);

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
        
        entity.User = await _context.Users.FindAsync(new object?[] { entity.CreatedBy },
            cancellationToken);

        var opinionDto = _mapper.Map<OpinionDto>(entity);

        return opinionDto;
    }
}