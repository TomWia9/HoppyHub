using MediatR;

namespace Application.Users.Commands.UpdateUser;

/// <summary>
///     UpdateUser command. 
/// </summary>
public record UpdateUserCommand : IRequest
{
    /// <summary>
    ///     The user id.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    ///     The username.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    ///     The user current password.
    /// </summary>
    public string? CurrentPassword { get; init; }

    /// <summary>
    ///     The user new password.
    /// </summary>
    public string? NewPassword { get; init; }
}