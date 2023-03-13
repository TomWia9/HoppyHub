using Application.Beers;
using Application.Beers.Commands.CreateBeer;
using Application.Beers.Commands.DeleteBeer;
using Application.Beers.Commands.UpdateBeer;
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
    ///     Creates the beer.
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

    /// <summary>
    ///     Updates the beer.
    /// </summary>
    /// <param name="id">The beer id</param>
    /// <param name="command">The UpdateBeerCommand</param>
    /// <returns>An ActionResult</returns>
    [Authorize(Policy = Policies.AdministratorAccess)]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BeerDto>> UpdateBeer(Guid id, [FromBody] UpdateBeerCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest();
        }

        await Mediator.Send(command);

        return NoContent();
    }
    
    /// <summary>
    ///     Deletes the beer.
    /// </summary>
    /// <param name="id">The beer id</param>
    /// <returns>An ActionResult</returns>
    [Authorize(Policy = Policies.AdministratorAccess)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<BeerDto>> DeleteBeer(Guid id)
    {
        await Mediator.Send(new DeleteBeerCommand { Id = id });

        return NoContent();
    }
    
    /// <summary>
    ///     Gets beer by id.
    /// </summary>
    /// <param name="id">The beer id</param>
    /// <returns>An ActionResult of type BeerDto</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BeerDto>> GetBeer(Guid id)
    {
        // TODO Add implementation
        throw new NotImplementedException();
    }
}