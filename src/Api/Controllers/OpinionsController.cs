using Application.Opinions.Dtos;
using Application.Opinions.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
///     The opinions controller.
/// </summary>
public class OpinionsController : ApiControllerBase
{
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