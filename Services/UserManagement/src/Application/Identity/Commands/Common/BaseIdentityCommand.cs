namespace Application.Identity.Commands.Common;

/// <summary>
///     BaseIdentityCommand abstract record.
/// </summary>
public abstract record BaseIdentityCommand
{
    /// <summary>
    ///     The user email.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    ///     The password.
    /// </summary>
    public string? Password { get; init; }
}