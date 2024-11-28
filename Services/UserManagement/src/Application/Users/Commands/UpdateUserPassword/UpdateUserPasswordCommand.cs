using MediatR;

namespace Application.Users.Commands.UpdateUserPassword;

/// <summary>
///     UpdateUserPassword command.
/// </summary>
public record UpdateUserPasswordCommand : IRequest
{
    /// <summary>
    ///     The user id.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    ///     The user current password.
    /// </summary>
    public string? CurrentPassword { get; init; }
    
    /// <summary>
    ///     The user new password.
    /// </summary>
    public string? NewPassword { get; init; }
}