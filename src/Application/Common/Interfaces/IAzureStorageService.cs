using Application.Common.Models.BlobContainer;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces;

/// <summary>
///     The AzureStorageService interface.
/// </summary>
public interface IAzureStorageService
{
    /// <summary>
    ///     Uploads a file submitted with the request.
    /// </summary>
    /// <param name="path">The blob path</param>
    /// <param name="blob">The blob for upload</param>
    /// <returns>BlobResponseDto with status</returns>
    Task<BlobResponseDto> UploadAsync(string path, IFormFile blob);

    /// <summary>
    ///     Deletes a blob on a given path.
    /// </summary>
    /// <param name="path">The blob path</param>
    /// <returns>BlobResponseDto with status</returns>
    Task<BlobResponseDto> DeleteAsync(string path);

    /// <summary>
    ///     Deletes all files in given path.
    /// </summary>
    /// <param name="path">The path</param>
    Task DeleteFilesInPath(string path);
}