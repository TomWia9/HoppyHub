namespace SharedEvents;

/// <summary>
///     The user deleted event.
/// </summary>
public record UserDeleted
{
    /// <summary>
    ///     The user id.
    /// </summary>
    public Guid Id { get; set; }
}