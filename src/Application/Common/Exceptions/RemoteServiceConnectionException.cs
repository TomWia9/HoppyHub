namespace Application.Common.Exceptions;

/// <summary>
///     RemoteServiceConnectionException class.
/// </summary>
public class RemoteServiceConnectionException : Exception
{
    /// <summary>
    ///     Initializes RemoteServiceConnectionException with message.
    /// </summary>
    /// <param name="message">The message</param>
    public RemoteServiceConnectionException(string message) : base(message)
    {
    }
}