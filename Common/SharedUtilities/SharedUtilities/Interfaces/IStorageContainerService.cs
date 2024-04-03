using Microsoft.AspNetCore.Http;

namespace SharedUtilities.Interfaces;

/// <summary>
///     StorageContainer service interface.
/// </summary>
public interface IStorageContainerService
{
    /// <summary>
    ///     Uploads file async.
    /// </summary>
    /// <param name="fileName">The file</param>
    /// <param name="file">The file name</param>
    /// <returns>File uri</returns>
    public Task<string?> UploadAsync(string fileName, IFormFile file);

    /// <summary>
    ///     Deletes files from given path.
    /// </summary>
    /// <param name="path">The path</param>
    public Task DeleteFromPathAsync(string path);
}