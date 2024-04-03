using Application.Common.Interfaces;
using Application.Opinions.Dtos;
using AutoMapper;
using Domain.Entities;
using MediatR;
using SharedUtilities.Exceptions;
using SharedUtilities.Interfaces;

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
    ///     The opinions service.
    /// </summary>
    private readonly IOpinionsService _opinionsService;

    /// <summary>
    ///     The storage container service.
    /// </summary>
    private readonly IStorageContainerService _storageContainerService;

    /// <summary>
    ///     Initializes CreateOpinionCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="mapper">The mapper</param>
    /// <param name="opinionsService">TThe opinions service</param>
    /// <param name="storageContainerService">The storage container service</param>
    public CreateOpinionCommandHandler(IApplicationDbContext context, IMapper mapper, IOpinionsService opinionsService,
        IStorageContainerService storageContainerService)
    {
        _context = context;
        _mapper = mapper;
        _opinionsService = opinionsService;
        _storageContainerService = storageContainerService;
    }

    /// <summary>
    ///     Handles CreateOpinionCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<OpinionDto> Handle(CreateOpinionCommand request, CancellationToken cancellationToken)
    {
        var beer = await _context.Beers.FindAsync([request.BeerId], cancellationToken);

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
                var fileName =
                    $"Opinions/{beer.BreweryId.ToString()}/{beer.Id.ToString()}/{entity.Id.ToString()}{Path.GetExtension(request.Image.FileName)}";

                var imageUri = await _storageContainerService.UploadAsync(fileName, request.Image);

                if (string.IsNullOrEmpty(imageUri))
                {
                    throw new RemoteServiceConnectionException("Failed to upload image.");
                }

                entity.ImageUri = imageUri;
                await _context.SaveChangesAsync(cancellationToken);
            }
            
            await transaction.CommitAsync(cancellationToken);
            
            await _opinionsService.PublishOpinionChangedEventAsync(beer.Id, cancellationToken);

        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        entity.User = await _context.Users.FindAsync([entity.CreatedBy],
            cancellationToken);

        var opinionDto = _mapper.Map<OpinionDto>(entity);

        return opinionDto;
    }
}