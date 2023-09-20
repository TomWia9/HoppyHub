namespace SharedUtilities.Exceptions;

/// <summary>
///     NotFoundException class.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    ///     Initializes NotFoundException with message.
    /// </summary>
    /// <param name="message">The message</param>
    public NotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes NotFoundException with message based on entity name and key.
    /// </summary>
    /// <param name="name">The entity name</param>
    /// <param name="key">The entity key</param>
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }
}