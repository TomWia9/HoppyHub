using Application.BeerStyles.Commands.CreateBeerStyle;
using Application.BeerStyles.Commands.DeleteBeerStyle;
using Application.BeerStyles.Commands.UpdateBeerStyle;
using Application.BeerStyles.Dtos;
using Application.BeerStyles.Queries.GetBeerStyle;
using Application.BeerStyles.Queries.GetBeerStyles;
using Application.Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
///     The beer styles controller.
/// </summary>
public class BeerStylesController : ApiControllerBase
{
    /// <summary>
    ///     Gets beer styles.
    /// </summary>
    /// <returns>An ActionResult of type PaginatedList of BeerStyleDto</returns>
    [HttpGet]
    public async Task<ActionResult<BeerStyleDto>> GetBeerStyles([FromQuery] GetBeerStylesQuery query)
    {
        var result = await Mediator.Send(query);

        Response.Headers.Add("X-Pagination", result.GetMetadata());

        return Ok(result);
    }

    /// <summary>
    ///     Gets beer style by id.
    /// </summary>
    /// <param name="id">The beer style id</param>
    /// <returns>An ActionResult of type BeerStyleDto</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BeerStyleDto>> GetBeerStyle(Guid id)
    {
        var result = await Mediator.Send(new GetBeerStyleQuery { Id = id });

        return Ok(result);
    }

    /// <summary>
    ///     Creates the beer style.
    /// </summary>
    /// <param name="command">The CreateBeerStyleCommand</param>
    /// <returns>An ActionResult of type BeerStyleDto</returns>
    [Authorize(Policy = Policies.AdministratorAccess)]
    [HttpPost]
    public async Task<ActionResult<BeerStyleDto>> CreateBeerStyle([FromBody] CreateBeerStyleCommand command)
    {
        var result = await Mediator.Send(command);

        return CreatedAtAction("GetBeerStyle", new { id = result.Id }, result);
    }

    /// <summary>
    ///     Updates the beer style.
    /// </summary>
    /// <param name="id">The beer style id</param>
    /// <param name="command">The UpdateBeerStyleCommand</param>
    /// <returns>An ActionResult</returns>
    [Authorize(Policy = Policies.AdministratorAccess)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBeerStyle(Guid id, [FromBody] UpdateBeerStyleCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(InvalidIdMessage);
        }

        await Mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    ///     Deletes the beer style.
    /// </summary>
    /// <param name="id">The beer style id</param>
    /// <returns>An ActionResult</returns>
    [Authorize(Policy = Policies.AdministratorAccess)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBeerStyle(Guid id)
    {
        await Mediator.Send(new DeleteBeerStyleCommand { Id = id });

        return NoContent();
    }
}