namespace Application.Common.Interfaces;

/// <summary>
///     The opinions service interface.
/// </summary>
public interface IOpinionsService
{
    /// <summary>
    ///     Publishes OpinionChanged event
    /// </summary>
    /// <param name="beerId">The beer id</param>
    /// <param name="cancellationToken">The cancellation token</param>
    Task PublishOpinionChangedEventAsync(Guid beerId, CancellationToken cancellationToken);
}