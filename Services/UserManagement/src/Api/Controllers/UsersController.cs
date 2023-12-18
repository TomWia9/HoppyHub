using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.UpdateUser;
using Application.Users.Dtos;
using Application.Users.Queries.GetUser;
using Application.Users.Queries.GetUsers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedUtilities.Models;

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
        var result = await Mediator.Send(new GetUserQuery { Id = id });

        return Ok(result);
    }

    /// <summary>
    ///     Gets users.
    /// </summary>
    /// <param name="query">The GetUsersQuery</param>
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetUsers([FromQuery] GetUsersQuery query)
    {
        var result = await Mediator.Send(query);

        Response.Headers.Append("X-Pagination", result.GetMetadata());

        return Ok(result);
    }

    /// <summary>
    ///     Updates user.
    /// </summary>
    /// <param name="id">The user id</param>
    /// <param name="command">The UpdateUserCommand</param>
    [Authorize(Policy = Policies.UserAccess)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.UserId)
        {
            return BadRequest(InvalidIdMessage);
        }

        await Mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    ///     Deletes user.
    /// </summary>
    /// <param name="id">The user id</param>
    /// <param name="command">The DeleteUserCommand</param>
    [Authorize(Policy = Policies.UserAccess)]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id, [FromBody] DeleteUserCommand command)
    {
        if (id != command.UserId)
        {
            return BadRequest(InvalidIdMessage);
        }

        await Mediator.Send(command);

        return NoContent();
    }
}