using System.ComponentModel.DataAnnotations;

namespace Azure.Comms.EmailRelay.Functions.Models;

/// <summary>
/// Points to a binary blob containing an attachment for the email message.
/// </summary>
public sealed class EmailAttachmentReference
{
    [Required]
    public string BlobContainerName { get; set; } = string.Empty;

    [Required]
    public string BlobName { get; set; } = string.Empty;

    /// <summary>
    /// Mime type for the attachment content. Provides hints for downstream processing.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Optional pre-computed length of the attachment, in bytes.
    /// </summary>
    [Range(0, long.MaxValue)]
    public long? ContentLength { get; set; }
}
