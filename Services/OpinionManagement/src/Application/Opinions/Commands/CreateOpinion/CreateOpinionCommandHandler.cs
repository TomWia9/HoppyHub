using Application.Common.Interfaces;
using Application.Opinions.Dtos;
using AutoMapper;
using Domain.Entities;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedEvents.Events;
using SharedEvents.Responses;
using SharedUtilities.Exceptions;
using SharedUtilities.Extensions;

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
    ///     The image created request client.
    /// </summary>
    private readonly IRequestClient<ImageCreated> _imageCreatedRequestClient;

    /// <summary>
    ///     The publish endpoint.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Initializes CreateOpinionCommandHandler.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="mapper">The mapper</param>
    /// <param name="imageCreatedRequestClient">The image created request client</param>
    /// <param name="publishEndpoint">The publish endpoint</param>
    public CreateOpinionCommandHandler(IApplicationDbContext context, IMapper mapper,
        IRequestClient<ImageCreated> imageCreatedRequestClient, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _mapper = mapper;
        _imageCreatedRequestClient = imageCreatedRequestClient;
        _publishEndpoint = publishEndpoint;
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
            
            await UploadImageAsync(entity, request.Image, beer.BreweryId, beer.Id, entity.Id, cancellationToken);
            await SendOpinionChangedEventAsync(beer.Id, cancellationToken);

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