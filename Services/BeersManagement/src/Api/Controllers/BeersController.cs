using Application.BeerImages.Commands.DeleteBeerImage;
using Application.BeerImages.Commands.UpsertBeerImage;
using Application.Beers.Commands.CreateBeer;
using Application.Beers.Commands.DeleteBeer;
using Application.Beers.Commands.UpdateBeer;
using Application.Beers.Dtos;
using Application.Beers.Queries.GetBeer;
using Application.Beers.Queries.GetBeers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedUtilities.Models;

namespace Api.Controllers;

/// <summary>
///     The beers controller.
/// </summary>
public class BeersController : ApiControllerBase
{
    /// <summary>
    ///     Gets beers.
    /// </summary>
    /// <param name="query">The GetBeersQuery</param>
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
    public async Task<IActionResult> UpdateBeer(Guid id, [FromBody] UpdateBeerCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(InvalidIdMessage);
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
    public async Task<IActionResult> DeleteBeer(Guid id)
    {
        await Mediator.Send(new DeleteBeerCommand { Id = id });

        return NoContent();
    }

    /// <summary>
    ///     Creates or updates the beer image.
    /// </summary>
    /// <param name="beerId">The beer id</param>
    /// <param name="command">The UpsertBeerImageCommand</param>
    /// <returns>An ActionResult of string with beer image uri</returns>
    [Authorize(Policy = Policies.AdministratorAccess)]
    [HttpPost("{beerId:guid}/upsertImage")]
    public async Task<IActionResult> UpsertBeerImage(Guid beerId, [FromForm] UpsertBeerImageCommand command)
    {
        if (beerId != command.BeerId)
        {
            return BadRequest(InvalidIdMessage);
        }

        await Mediator.Send(command);

        return Accepted();
    }

    /// <summary>
    ///     Deletes the beer image and restores the temp image.
    /// </summary>
    /// <param name="beerId">The beer id</param>
    /// <returns>An ActionResult</returns>
    [Authorize(Policy = Policies.AdministratorAccess)]
    [HttpDelete("{beerId:guid}/deleteImage")]
    public async Task<IActionResult> DeleteBeerImage(Guid beerId)
    {
        await Mediator.Send(new DeleteBeerImageCommand { BeerId = beerId });

        return Accepted();
    }
}