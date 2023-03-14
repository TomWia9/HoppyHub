namespace Application.Common.Extensions;

/// <summary>
///     The StringExtensions class.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    ///     Indicates whether source string contains substring with case insensitive.
    /// </summary>
    /// <param name="source">The source string</param>
    /// <param name="substring">The substring</param>
    public static bool ContainsCaseInsensitive(this string? source, string substring)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return false;
        }

        return source.IndexOf(substring, StringComparison.OrdinalIgnoreCase) > -1;
    }
}