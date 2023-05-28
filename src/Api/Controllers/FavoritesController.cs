using Application.Common.Models;
using Application.Favorites.Commands.CreateFavorite;
using Application.Favorites.Commands.DeleteFavorite;
using Application.Favorites.Dtos;
using Application.Favorites.Queries.GetFavorites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
///     The favorites controller.
/// </summary>
public class FavoritesController : ApiControllerBase
{
    /// <summary>
    ///     Gets favorite beers of a specific user.
    /// </summary>
    /// <returns>An ActionResult of type FavoritesListDto</returns>
    [HttpGet]
    public async Task<ActionResult<FavoritesListDto>> GetFavorites([FromQuery] GetFavoritesQuery query)
    {
        var result = await Mediator.Send(query);

        Response.Headers.Add("X-Pagination",  result.FavoriteBeers?.GetMetadata());

        return Ok(result);
    }
    
    /// <summary>
    ///     Adds beer to favorites.
    /// </summary>
    /// <param name="command">The CreateFavoriteCommand</param>
    /// <returns>An ActionResult</returns>
    [Authorize(Policy = Policies.UserAccess)]
    [HttpPost]
    public async Task<IActionResult> CreateFavorite([FromBody] CreateFavoriteCommand command)
    {
        await Mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    ///     Deletes the beer from favorites.
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