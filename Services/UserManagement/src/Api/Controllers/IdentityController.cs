// using Application.Common.Models;
// using Application.Identity.Commands.LoginUser;
// using Application.Identity.Commands.RegisterUser;
// using Microsoft.AspNetCore.Mvc;
// using UserManagement.Controllers;
//
// namespace Api.Controllers;
//
// /// <summary>
// ///     The identity controller.
// /// </summary>
// public class IdentityController : ApiControllerBase
// {
//     /// <summary>
//     ///     Registers the user.
//     /// </summary>
//     /// <param name="request">The register user request</param>
//     /// <returns>An ActionResult of AuthenticationResult</returns>
//     /// <response code="200">Creates the user and returns AuthenticationResult with token</response>
//     /// <response code="400">Bad request</response>
//     [HttpPost("register")]
//     public async Task<ActionResult<AuthenticationResult>> Register([FromBody] RegisterUserCommand request)
//     {
//         var result = await Mediator.Send(request);
//
//         return result.Succeeded ? Ok(result) : BadRequest(result);
//     }
//
//     /// <summary>
//     ///     Authenticates the user
//     /// </summary>
//     /// <param name="request">The user login request</param>
//     /// <returns>An ActionResult of AuthenticationResult</returns>
//     /// <response code="200">Authenticates the user and returns AuthenticationResult with token</response>
//     /// <response code="400">Bad request</response>
//     [HttpPost("login")]
//     public async Task<ActionResult<AuthenticationResult>> Login([FromBody] LoginUserCommand request)
//     {
//         var result = await Mediator.Send(request);
//
//         return result.Succeeded ? Ok(result) : BadRequest(result);
//     }
// }