namespace Application.Common.Interfaces;

/// <summary>
///     BeersService interface.
/// </summary>
public interface IBeersService
{
    /// <summary>
    ///     Calculates beer average rating asynchronously.
    /// </summary>
    /// <param name="beerId">The beer id</param>
    Task CalculateBeerRatingAsync(Guid beerId);
}