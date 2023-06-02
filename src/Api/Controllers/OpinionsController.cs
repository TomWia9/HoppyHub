using Application.BeerStyles.Dtos;
using Application.Common.Models;
using Application.Opinions.Commands.CreateOpinion;
using Application.Opinions.Commands.DeleteOpinion;
using Application.Opinions.Commands.UpdateOpinion;
using Application.Opinions.Dtos;
using Application.Opinions.Queries.GetOpinion;
using Application.Opinions.Queries.GetOpinions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
///     The opinions controller.
/// </summary>
public class OpinionsController : ApiControllerBase
{
    /// <summary>
    ///     Gets opinions.
    /// </summary>
    /// <param name="query">The GetOpinionsQuery</param>
    /// <returns>An ActionResult of type PaginatedList of OpinionDto</returns>
    [HttpGet]
    public async Task<ActionResult<OpinionDto>> GetOpinions([FromQuery] GetOpinionsQuery query)
    {
        var result = await Mediator.Send(query);

        Response.Headers.Add("X-Pagination", result.GetMetadata());

        return Ok(result);
    }

    /// <summary>
    ///     Gets opinion by id.
    /// </summary>
    /// <param name="id">The opinion id</param>
    /// <returns>An ActionResult of type OpinionDto</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OpinionDto>> GetOpinion(Guid id)
    {
        var result = await Mediator.Send(new GetOpinionQuery { Id = id });

        return Ok(result);
    }
    
    /// <summary>
    ///     Creates the opinion.
    /// </summary>
    /// <param name="command">The CreateOpinionCommand</param>
    /// <returns>An ActionResult of type OpinionDto</returns>
    [Authorize(Policy = Policies.UserAccess)]
    [HttpPost]
    public async Task<ActionResult<BeerStyleDto>> CreateOpinion([FromBody] CreateOpinionCommand command)
    {
        var result = await Mediator.Send(command);

        return CreatedAtAction("GetOpinion", new { id = result.Id }, result);
    }

    /// <summary>
    ///     Updates the opinion.
    /// </summary>
    /// <param name="id">The opinion id</param>
    /// <param name="command">The UpdateOpinionCommand</param>
    /// <returns>An ActionResult</returns>
    [Authorize(Policy = Policies.UserAccess)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateOpinion(Guid id, [FromBody] UpdateOpinionCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest(InvalidIdMessage);
        }

        await Mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    ///     Deletes the opinion.
    /// </summary>
    /// <param name="id">The opinion id</param>
    /// <returns>An ActionResult</returns>
    [Authorize(Policy = Policies.UserAccess)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteOpinion(Guid id)
    {
        await Mediator.Send(new DeleteOpinionCommand { Id = id });

        return NoContent();
    }
}