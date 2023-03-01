using MediatR;

namespace Application.Users.Commands.DeleteUser;

/// <summary>
///     DeleteUser command.
/// </summary>
public record DeleteUserCommand : IRequest
{
    /// <summary>
    ///     The user id.
    /// </summary>
    public Guid UserId { get; init; }
    
    /// <summary>
    ///    The user password.
    /// </summary>
    public string? Password { get; init; }
}