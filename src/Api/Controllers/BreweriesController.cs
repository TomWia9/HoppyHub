using Application.Breweries.Dtos;
using Application.Breweries.Queries.GetBreweries;
using Application.Breweries.Queries.GetBrewery;
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
    
    /// <summary>
    ///     Gets brewery by id.
    /// </summary>
    /// <param name="id">The brewery id</param>
    /// <returns>An ActionResult of type BreweryDto</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BreweryDto>> GetBeer(Guid id)
    {
        var result = await Mediator.Send(new GetBreweryQuery { Id = id });

        return Ok(result);
    }
}