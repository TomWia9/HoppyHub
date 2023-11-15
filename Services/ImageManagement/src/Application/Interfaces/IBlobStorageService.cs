using Application.Models.BlobContainer;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

/// <summary>
///     The AzureStorageService interface.
/// </summary>
public interface IBlobStorageService
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
    ///     Deletes a blob by blob uri.
    /// </summary>
    /// <param name="uri">The blob uri</param>
    /// <returns>BlobResponseDto with status</returns>
    Task<BlobResponseDto> DeleteByUriAsync(string uri);

    /// <summary>
    ///     Deletes all blobs in given path.
    /// </summary>
    /// <param name="path">The path</param>
    Task DeleteInPath(string path);
}