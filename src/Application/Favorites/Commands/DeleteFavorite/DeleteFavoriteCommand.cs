using MediatR;

namespace Application.Favorites.Commands.DeleteFavorite;

/// <summary>
///     DeleteFavorite command.
/// </summary>
public record DeleteFavoriteCommand : IRequest
{
    /// <summary>
    ///     The added to favorites beer id.
    /// </summary>
    public Guid BeerId { get; init; }
}