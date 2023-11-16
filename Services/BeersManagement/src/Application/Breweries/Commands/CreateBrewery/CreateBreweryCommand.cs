using Application.Breweries.Commands.Common;
using Application.Breweries.Dtos;
using MediatR;

namespace Application.Breweries.Commands.CreateBrewery;

/// <summary>
///     CreateBrewery command.
/// </summary>
public record CreateBreweryCommand : BaseBreweryCommand, IRequest<BreweryDto>;