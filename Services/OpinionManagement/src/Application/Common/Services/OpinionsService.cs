using Application.Common.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using SharedEvents.Events;

namespace Application.Common.Services;

/// <summary>
///     The opinions service.
/// </summary>
public class OpinionsService : IOpinionsService
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     The publish endpoint.
    /// </summary>
    private readonly IPublishEndpoint _publishEndpoint;

    /// <summary>
    ///     Initializes OpinionsService.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="publishEndpoint">The publish endpoint</param>
    public OpinionsService(IApplicationDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    /// <summary>
    ///     Publishes OpinionChanged event
    /// </summary>
    /// <param name="beerId">The beer id</param>
    /// <param name="cancellationToken">The cancellation token</param>
    public async Task PublishOpinionChangedEventAsync(Guid beerId, CancellationToken cancellationToken)
    {
        var beerOpinions = _context.Opinions.Where(x => x.BeerId == beerId);
        var newBeerOpinionsCount = await beerOpinions.CountAsync(cancellationToken);
        var newBeerRating = !await beerOpinions.AnyAsync(cancellationToken)
            ? 0
            : await beerOpinions.AverageAsync(x => x.Rating, cancellationToken);
        var beerOpinionChanged = new BeerOpinionChanged
        {
            BeerId = beerId,
            OpinionsCount = newBeerOpinionsCount,
            NewBeerRating = Math.Round(newBeerRating, 2)
        };

        await _publishEndpoint.Publish(beerOpinionChanged, cancellationToken);
    }
}