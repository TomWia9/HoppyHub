using MediatR;

namespace Application.Users.Commands.UpdateUsername;

/// <summary>
///     UpdateUsername command.
/// </summary>
public record UpdateUsernameCommand : IRequest
{
    /// <summary>
    ///     The user id.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    ///     The username.
    /// </summary>
    public string? Username { get; init; }
}