using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
///     The ApiControllerBase abstract class.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    ///     The invalid id error message.
    /// </summary>
    protected const string InvalidIdMessage = "The ID in the route differs from the ID in the request body.";

    /// <summary>
    ///     The mediator.
    /// </summary>
    private ISender? _mediator;

    /// <summary>
    ///     The mediator.
    /// </summary>
    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}