using Application.BeerImages.Commands.UpsertBeerImage;
using Application.Beers.Commands.CreateBeer;
using Application.Beers.Commands.DeleteBeer;
using Application.Beers.Commands.UpdateBeer;
using Application.Beers.Dtos;
using Application.Beers.Queries.GetBeer;
using Application.Beers.Queries.GetBeers;
using Application.Common.Models;
using Application.Favorites.Commands.CreateFavorite;
using Application.Favorites.Commands.DeleteFavorite;
using Application.Favorites.Queries.GetFavorites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    [HttpPost("{beerId:guid}")]
    public async Task<IActionResult> UpsertBeerImage(Guid beerId, [FromForm] UpsertBeerImageCommand command)
    {
        if (beerId != command.BeerId)
        {
            return BadRequest(InvalidIdMessage);
        }

        var result = await Mediator.Send(command);

        return CreatedAtAction("GetBeer", new { id = beerId }, result);
    }

    /// <summary>
    ///     Gets favorite beers of a specific user.
    /// </summary>
    /// <param name="query">The GetFavoritesQuery</param>
    /// <returns>An ActionResult of type PaginatedList of BeerDto</returns>
    [HttpGet("favorites")]
    public async Task<ActionResult<PaginatedList<BeerDto>>> GetFavorites([FromQuery] GetFavoritesQuery query)
    {
        var result = await Mediator.Send(query);

        Response.Headers.Add("X-Pagination", result.GetMetadata());

        return Ok(result);
    }

    /// <summary>
    ///     Adds beer to favorites.
    /// </summary>
    /// <param name="beerId">The beer id.</param>
    /// <returns>An ActionResult</returns>
    [Authorize(Policy = Policies.UserAccess)]
    [HttpPost("{beerId:guid}/favorites")]
    public async Task<IActionResult> CreateFavorite(Guid beerId)
    {
        await Mediator.Send(new CreateFavoriteCommand { BeerId = beerId });

        return NoContent();
    }

    /// <summary>
    ///     Deletes the beer from favorites.
    /// </summary>
    /// <param name="beerId">The id of the beer added to favorites</param>
    /// <returns>An ActionResult</returns>
    [Authorize(Policy = Policies.UserAccess)]
    [HttpDelete("{beerId:guid}/favorites")]
    public async Task<IActionResult> DeleteFavorite(Guid beerId)
    {
        await Mediator.Send(new DeleteFavoriteCommand { BeerId = beerId });

        return NoContent();
    }
}