using System.ComponentModel.DataAnnotations;

namespace Azure.Comms.EmailRelay.Functions.Configuration;

/// <summary>
/// Options describing where attachments and email artifacts are stored.
/// </summary>
public sealed class BlobStorageOptions
{
    public const string SectionName = "AttachmentStorage";

    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    [Required]
    public string ContainerName { get; set; } = "email-attachments";

    [Required]
    public string LogsContainerName { get; set; } = "email-logs";
}
