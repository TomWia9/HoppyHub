using Microsoft.AspNetCore.Http;

namespace SharedUtilities.Extensions;

/// <summary>
///     FormFile extensions class.
/// </summary>
public static class FormFileExtensions
{
    /// <summary>
    ///     Converts FormFile to bytes.
    /// </summary>
    /// <param name="formFile">The FormFile</param>
    public static async Task<byte[]> GetBytes(this IFormFile formFile)
    {
        await using var memoryStream = new MemoryStream();
        await formFile.CopyToAsync(memoryStream);

        return memoryStream.ToArray();
    }
}