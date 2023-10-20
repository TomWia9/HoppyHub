namespace SharedEvents;

/// <summary>
///     The user created event.
/// </summary>
public record UserCreated
{
    /// <summary>
    ///     The user id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     The email.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    ///     The username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    ///     The role.
    /// </summary>
    public string? Role { get; set; }
}