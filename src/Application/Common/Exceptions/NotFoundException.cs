namespace Application.Common.Exceptions;

/// <summary>
///     NotFoundException class.
/// </summary>
public class NotFoundException : Exception
{
    /// <summary>
    ///     Initializes NotFoundException.
    /// </summary>
    public NotFoundException() : base()
    {
    }

    /// <summary>
    ///     Initializes NotFoundException with message.
    /// </summary>
    /// <param name="message">The message</param>
    public NotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes NotFoundException with message and inner exception.
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="innerException">The inner exception</param>
    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
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