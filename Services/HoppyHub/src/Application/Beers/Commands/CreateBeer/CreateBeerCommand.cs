using Application.Beers.Commands.Common;
using Application.Beers.Dtos;
using MediatR;

namespace Application.Beers.Commands.CreateBeer;

/// <summary>
///     CreateBeer command.
/// </summary>
public record CreateBeerCommand : BaseBeerCommand, IRequest<BeerDto>;