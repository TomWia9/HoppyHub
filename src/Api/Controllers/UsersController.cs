using Application.Users.Queries;
using Application.Users.Queries.GetUser;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
///     The users controller.
/// </summary>
public class UsersController : ApiControllerBase
{
    /// <summary>
    ///     Gets user by id.
    /// </summary>
    /// <param name="id">The user id</param>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetUser(Guid id)
    {
        var result = await Mediator.Send(new GetUserQuery { UserId = id });

        return Ok(result);
    }
}