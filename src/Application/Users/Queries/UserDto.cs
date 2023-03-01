namespace Application.Users.Queries;

/// <summary>
///     UserDto record.
/// </summary>
public record UserDto
{
    /// <summary>
    ///     The user id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     The email.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    ///     The username.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    ///     The role.
    /// </summary>
    public string? Role { get; init; }
}