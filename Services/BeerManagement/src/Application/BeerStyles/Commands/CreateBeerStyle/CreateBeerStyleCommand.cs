using Application.BeerStyles.Commands.Common;
using Application.BeerStyles.Dtos;
using MediatR;

namespace Application.BeerStyles.Commands.CreateBeerStyle;

/// <summary>
///     CreateBeerStyle command.
/// </summary>
public record CreateBeerStyleCommand : BaseBeerStyleCommand, IRequest<BeerStyleDto>;