using Application.Common.Models;
using Application.Users.Commands.UpdateUser;
using Application.Users.Queries;
using Application.Users.Queries.GetUser;
using Application.Users.Queries.GetUsers;
using Microsoft.AspNetCore.Authorization;
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

    /// <summary>
    ///     Gets users.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<UserDto>> GetUsers([FromQuery] GetUsersQuery query)
    {
        var result = await Mediator.Send(query);

        Response.Headers.Add("X-Pagination", result.GetMetadata());

        return Ok(result);
    }

    /// <summary>
    ///     Updates user.
    /// </summary>
    [Authorize(Policy = Policies.UserAccess)]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.UserId)
        {
            return BadRequest();
        }

        await Mediator.Send(command);

        return NoContent();
    }
}