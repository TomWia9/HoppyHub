using MediatR;

namespace Application.Beers.Commands.DeleteBeer;

/// <summary>
///     DeleteBeer command
/// </summary>
public record DeleteBeerCommand : IRequest
{
    public Guid Id { get; init; }
}