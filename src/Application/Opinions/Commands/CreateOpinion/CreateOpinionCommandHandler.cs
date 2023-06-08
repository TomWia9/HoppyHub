using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models.BlobContainer;
using Application.Opinions.Dtos;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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
    ///     The azure storage service.
    /// </summary>
    private readonly IAzureStorageService _azureStorageService;

    /// <summary>
    ///     The current user service.
    /// </summary>
    private readonly ICurrentUserService _currentUserService;

    /// <summary>
    ///     Initializes CreateOpinionCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="mapper">The mapper</param>
    /// <param name="usersService">The users service</param>
    /// <param name="beersService">The beers service</param>
    /// <param name="azureStorageService">The azure storage service</param>
    /// <param name="currentUserService">The current user service.</param>
    public CreateOpinionCommandHandler(IApplicationDbContext context, IMapper mapper, IUsersService usersService,
        IBeersService beersService, IAzureStorageService azureStorageService, ICurrentUserService currentUserService)
    {
        _context = context;
        _mapper = mapper;
        _usersService = usersService;
        _beersService = beersService;
        _azureStorageService = azureStorageService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    ///     Handles CreateOpinionCommand.
    /// </summary>
    /// <param name="request">The request</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task<OpinionDto> Handle(CreateOpinionCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Beers.AnyAsync(x => x.Id == request.BeerId, cancellationToken))
        {
            throw new NotFoundException(nameof(Beer), request.BeerId);
        }

        var blobResponse = new BlobResponseDto();

        if (request.Image != null)
        {
            var path = CreateImagePath(request.Image, request.BeerId);

            blobResponse = await _azureStorageService.UploadAsync(path, request.Image);

            if (blobResponse.Error)
            {
                throw new RemoteServiceConnectionException("Failed to upload the photo. The opinion was not saved.");
            }
        }

        var entity = new Opinion
        {
            Rating = request.Rating,
            Comment = request.Comment,
            BeerId = request.BeerId,
            ImageUri = request.Image != null ? blobResponse.Blob.Uri : null
        };

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _context.Opinions.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
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
        opinionDto.Username = await _usersService.GetUsernameAsync(opinionDto.CreatedBy!.Value);

        return opinionDto;
    }

    /// <summary>
    ///     Returns file with name changed to match the folder structure in container "Opinions/BeerId/UserId.jpg/png"
    /// </summary>
    /// <param name="file">The file</param>
    /// <param name="beerId">The beer id</param>
    private string CreateImagePath(IFormFile file, Guid beerId)
    {
        var extension = Path.GetExtension(file.FileName);
        var userId = _currentUserService.UserId.ToString();

        return $"Opinions/{beerId.ToString()}/{userId}" + extension;
    }
}