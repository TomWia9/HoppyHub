using Application.Common.Models;
using Application.Favorites.Commands.CreateFavorite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
///     The favorites controller.
/// </summary>
public class FavoritesController : ApiControllerBase
{
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
}