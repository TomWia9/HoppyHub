using MediatR;

namespace Application.Favorites.Commands.DeleteFavorite;

/// <summary>
///     DeleteFavorite command.
/// </summary>
public record DeleteFavoriteCommand : IRequest
{
    /// <summary>
    ///     The favorite id.
    /// </summary>
    public Guid Id { get; set; }
}