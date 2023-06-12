using MediatR;

namespace Application.BeerImages.Commands.DeleteBeerImage;

/// <summary>
///     DeleteBeerImage command
/// </summary>
public record DeleteBeerImageCommand : IRequest
{
    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid BeerId { get; init; }
}