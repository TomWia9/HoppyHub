using Application.BeerStyles.Dtos;
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
}