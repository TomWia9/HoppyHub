namespace Application.Common.Models;

/// <summary>
///     AuthenticationResult class.
/// </summary>
public class AuthenticationResult
{
    /// <summary>
    ///     Initializes AuthenticationResult.
    /// </summary>
    /// <param name="succeeded">Indicates whether result succeeded</param>
    /// <param name="errors">The errors</param>
    /// <param name="token">The authentication token</param>
    public AuthenticationResult(bool succeeded, IEnumerable<string> errors, string? token)
    {
        Succeeded = succeeded;
        Errors = errors.ToArray();
        Token = token;
    }

    /// <summary>
    ///     Authentication token.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    ///     Indicates whether result succeeded.
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    ///     The errors.
    /// </summary>
    public string[] Errors { get; set; }

    /// <summary>
    ///     Returns Success result.
    /// </summary>
    public static AuthenticationResult Success(string token)
    {
        return new AuthenticationResult(true, Array.Empty<string>(), token);
    }

    /// <summary>
    ///     Returns Failure result.
    /// </summary>
    /// <param name="errors">The errors</param>
    public static AuthenticationResult Failure(IEnumerable<string> errors)
    {
        return new AuthenticationResult(false, errors, string.Empty);
    }
}