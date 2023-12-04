using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces;

/// <summary>
///     The opinions service interface.
/// </summary>
public interface IOpinionsService
{
    /// <summary>
    ///     Uploads image.
    /// </summary>
    /// <param name="entity">The opinion entity</param>
    /// <param name="image">The image</param>
    /// <param name="breweryId">The brewery id</param>
    /// <param name="beerId">The beer id</param>
    /// <param name="opinionId">The opinion id</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task UploadImageAsync(Opinion entity, IFormFile? image, Guid breweryId, Guid beerId, Guid opinionId,
        CancellationToken cancellationToken);

    /// <summary>
    ///     Deletes the image.
    /// </summary>
    /// <param name="uri">The image uri</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task DeleteImageAsync(string? uri, CancellationToken cancellationToken);

    /// <summary>
    ///     Sends OpinionChanged event
    /// </summary>
    /// <param name="beerId">The beer id</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task SendOpinionChangedEventAsync(Guid beerId, CancellationToken cancellationToken);
}