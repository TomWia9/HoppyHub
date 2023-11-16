using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.BeerImages.Commands.UpsertBeerImage;

/// <summary>
///     UpsertBeerImage command.
/// </summary>
public record UpsertBeerImageCommand : IRequest<string>
{
    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid BeerId { get; init; }

    /// <summary>
    ///     The beer image.
    /// </summary>
    public IFormFile? Image { get; init; }
}