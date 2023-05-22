using Application.BeerStyles.Dtos;
using Application.BeerStyles.Queries.GetBeerStyle;
using Application.BeerStyles.Queries.GetBeerStyles;
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
}