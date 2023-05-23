using Application.Opinions.Dtos;
using Application.Opinions.Queries.GetOpinion;
using Application.Opinions.Queries.GetOpinions;
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
}