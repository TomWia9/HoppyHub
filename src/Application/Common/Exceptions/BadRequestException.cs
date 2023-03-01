namespace Application.Common.Exceptions;

/// <summary>
///     BadRequestException class.
/// </summary>
public class BadRequestException : Exception
{
    /// <summary>
    ///     Initializes BadRequestException
    /// </summary>
    public BadRequestException()
    {
    }

    /// <summary>
    ///     Initializes BadRequestException with message
    /// </summary>
    /// <param name="message">The message</param>
    public BadRequestException(string message) : base(message)
    {
    }
}