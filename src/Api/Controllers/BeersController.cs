using Application.Beers;
using Application.Beers.Commands.CreateBeer;
using Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
///     The beers controller.
/// </summary>
public class BeersController : ApiControllerBase
{
    /// <summary>
    ///     Creates beer.
    /// </summary>
    /// <param name="command">The CreateBeerCommand</param>
    /// <returns>An ActionResult of type BeerDto</returns>
    [Authorize(Policy = Policies.AdministratorAccess)]
    [HttpPost]
    public async Task<ActionResult<BeerDto>> CreateBeer([FromBody] CreateBeerCommand command)
    {
        var result = await Mediator.Send(command);

        return CreatedAtAction("GetBeer", new { id = result.Id }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BeerDto>> GetBeer(Guid id)
    {
        // TODO Add implementation
        throw new NotImplementedException();
    }
}