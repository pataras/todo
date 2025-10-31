using System.ComponentModel.DataAnnotations;

namespace Azure.Comms.EmailRelay.Functions.Models;

/// <summary>
/// Canonical payload used when enqueuing emails onto the outbox queue.
/// </summary>
public sealed class EmailMessage
{
    /// <summary>
    /// Unique identifier for the message, defaults to a generated GUID.
    /// </summary>
    [Required]
    public string MessageId { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>
    /// Logical subject line shown to recipients.
    /// </summary>
    [Required]
    [MaxLength(2048)]
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// Content envelope containing plain text, HTML, or template references.
    /// </summary>
    [Required]
    public EmailContent Content { get; set; } = new();

    /// <summary>
    /// Recipients for the message across the To, Cc, and Bcc lists.
    /// </summary>
    [MinLength(1)]
    public IList<EmailRecipient> Recipients { get; set; } = new List<EmailRecipient>();

    /// <summary>
    /// Optionally instructs the dispatcher to use a specific sender instead of the default.
    /// </summary>
    public string? SenderOverride { get; set; }

    /// <summary>
    /// Optional reply-to addresses.
    /// </summary>
    public IList<EmailRecipient> ReplyTo { get; set; } = new List<EmailRecipient>();

    /// <summary>
    /// References to attachments stored in blob storage.
    /// </summary>
    public IList<EmailAttachmentReference> Attachments { get; set; } = new List<EmailAttachmentReference>();

    /// <summary>
    /// Operational metadata to support monitoring and retries.
    /// </summary>
    public EmailDispatchMetadata Metadata { get; set; } = new();
}
