namespace SharedEvents;

/// <summary>
///     The user updated event.
/// </summary>
public record UserUpdated
{
    /// <summary>
    ///     The user id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     The username.
    /// </summary>
    public string? Username { get; set; }
}