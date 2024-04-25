using Application.Favorites.Commands.CreateFavorite;
using Application.Favorites.Commands.DeleteFavorite;
using Application.Favorites.Dtos;
using Application.Favorites.Queries.GetFavorites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedUtilities.Models;

namespace Api.Controllers;

/// <summary>
///     The favorites controller.
/// </summary>
public class FavoritesController : ApiControllerBase
{
    /// <summary>
    ///     Gets favorite beers of a specific user.
    /// </summary>
    /// <param name="query">The GetFavoritesQuery</param>
    /// <returns>An ActionResult of type PaginatedList of BeerDto</returns>
    [HttpGet]
    public async Task<ActionResult<PaginatedList<BeerDto>>> GetFavorites([FromQuery] GetFavoritesQuery query)
    {
        var result = await Mediator.Send(query);

        Response.Headers.Append("X-Pagination", result.GetMetadata());

        return Ok(result);
    }

    /// <summary>
    ///     Adds beer to current user favorites list.
    /// </summary>
    /// <param name="beerId">The beer id.</param>
    /// <returns>An ActionResult</returns>
    [Authorize(Policy = Policies.UserAccess)]
    [HttpPost("{beerId:guid}")]
    public async Task<IActionResult> CreateFavorite(Guid beerId)
    {
        await Mediator.Send(new CreateFavoriteCommand { BeerId = beerId });

        return NoContent();
    }

    /// <summary>
    ///     Deletes the beer from current user favorites list.
    /// </summary>
    /// <param name="beerId">The id of the beer added to favorites</param>
    /// <returns>An ActionResult</returns>
    [Authorize(Policy = Policies.UserAccess)]
    [HttpDelete("{beerId:guid}")]
    public async Task<IActionResult> DeleteFavorite(Guid beerId)
    {
        await Mediator.Send(new DeleteFavoriteCommand { BeerId = beerId });

        return NoContent();
    }
}