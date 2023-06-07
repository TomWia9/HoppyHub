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
    /// <param name="blob">The blob for upload</param>
    /// <returns>BlobResponseDto with status</returns>
    Task<BlobResponseDto> UploadAsync(IFormFile blob);

    /// <summary>
    ///     Deletes a blob with the specified filename.
    /// </summary>
    /// <param name="blobFilename">Filename</param>
    /// <returns>BlobResponseDto with status</returns>
    Task<BlobResponseDto> DeleteAsync(string blobFilename);
}