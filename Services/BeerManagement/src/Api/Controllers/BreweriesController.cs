using Application.Breweries.Commands.CreateBrewery;
using Application.Breweries.Commands.DeleteBrewery;
using Application.Breweries.Commands.UpdateBrewery;
using Application.Breweries.Dtos;
using Application.Breweries.Queries.GetBreweries;
using Application.Breweries.Queries.GetBrewery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedUtilities.Models;

namespace Api.Controllers;

public class BreweriesController : ApiControllerBase
{
    /// <summary>
    ///     Gets breweries.
    /// </summary>
    /// <param name="query">The GetBreweriesQuery</param>
    /// <returns>An ActionResult of type PaginatedList of BreweryDto</returns>
    [HttpGet]
    public async Task<ActionResult<BreweryDto>> GetBreweries([FromQuery] GetBreweriesQuery query)
    {
        var result = await Mediator.Send(query);

        Response.Headers.Append("X-Pagination", result.GetMetadata());

        return Ok(result);
    }

    /// <summary>
    ///     Gets brewery by id.
    /// </summary>
    /// <param name="id">The brewery id</param>
    /// <returns>An ActionResult of type BreweryDto</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BreweryDto>> GetBrewery(Guid id)
    {
        var result = await Mediator.Send(new GetBreweryQuery { Id = id });

        return Ok(result);
    }

    /// <summary>
    ///     Creates the brewery.
    /// </summary>
    /// <param name="command">The CreateBreweryCommand</param>
    /// <returns>An ActionResult of type BreweryDto</returns>
    [Authorize(Policy = Policies.AdministratorAccess)]
    [HttpPost]
    public async Task<ActionResult<BreweryDto>> CreateBrewery([FromBody] CreateBreweryCommand command)
    {
        var result = await Mediator.Send(command);

        return CreatedAtAction("GetBrewery", new { id = result.Id }, result);
    }

    /// <summary>
    ///     Updates the brewery.
    /// </summary>
    /// <param name="id">The brewery id</param>
    /// <param name="command">The UpdateBreweryCommand</param>
    /// <returns>An ActionResult</returns>
    [Authorize(Policy = Policies.AdministratorAccess)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBrewery(Guid id, [FromBody] UpdateBreweryCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(InvalidIdMessage);
        }

        await Mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    ///     Deletes the brewery.
    /// </summary>
    /// <param name="id">The brewery id</param>
    /// <returns>An ActionResult</returns>
    [Authorize(Policy = Policies.AdministratorAccess)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBrewery(Guid id)
    {
        await Mediator.Send(new DeleteBreweryCommand { Id = id });

        return NoContent();
    }
}