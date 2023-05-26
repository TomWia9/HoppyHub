using MediatR;

namespace Application.Favorites.Commands.CreateFavorite;

/// <summary>
///     CreateFavorite command.
/// </summary>
public record CreateFavoriteCommand : IRequest
{
    /// <summary>
    ///     The favorite beer id.
    /// </summary>
    public Guid BeerId { get; set; }
}