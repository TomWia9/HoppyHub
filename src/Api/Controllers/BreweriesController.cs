using Application.Breweries.Dtos;
using Application.Breweries.Queries.GetBreweries;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class BreweriesController : ApiControllerBase
{
    /// <summary>
    ///     Gets breweries.
    /// </summary>
    /// <returns>An ActionResult of type PaginatedList of BreweryDto</returns>
    [HttpGet]
    public async Task<ActionResult<BreweryDto>> GetBeers([FromQuery] GetBreweriesQuery query)
    {
        var result = await Mediator.Send(query);

        Response.Headers.Add("X-Pagination", result.GetMetadata());

        return Ok(result);
    }
}