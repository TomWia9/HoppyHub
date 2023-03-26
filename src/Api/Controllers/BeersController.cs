using Application.Beers.Commands.CreateBeer;
using Application.Beers.Commands.DeleteBeer;
using Application.Beers.Commands.UpdateBeer;
using Application.Beers.Dtos;
using Application.Beers.Queries.GetBeer;
using Application.Beers.Queries.GetBeers;
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
    ///     Gets beers.
    /// </summary>
    /// <returns>An ActionResult of type PaginatedList of BeerDto</returns>
    [HttpGet]
    public async Task<ActionResult<BeerDto>> GetBeers([FromQuery] GetBeersQuery query)
    {
        var result = await Mediator.Send(query);

        Response.Headers.Add("X-Pagination", result.GetMetadata());

        return Ok(result);
    }

    /// <summary>
    ///     Gets beer by id.
    /// </summary>
    /// <param name="id">The beer id</param>
    /// <returns>An ActionResult of type BeerDto</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BeerDto>> GetBeer(Guid id)
    {
        var result = await Mediator.Send(new GetBeerQuery { Id = id });

        return Ok(result);
    }

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
}