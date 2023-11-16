namespace Application.Models.BlobContainer;

/// <summary>
///     The blob response data transfer object.
/// </summary>
public class BlobResponseDto
{
    /// <summary>
    ///     Initializes BlobResponseDto.
    /// </summary>
    public BlobResponseDto()
    {
        Blob = new BlobDto();
    }

    /// <summary>
    ///     The status.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    ///     Indicates error.
    /// </summary>
    public bool Error { get; set; }

    /// <summary>
    ///     The blob.
    /// </summary>
    public BlobDto Blob { get; set; }
}