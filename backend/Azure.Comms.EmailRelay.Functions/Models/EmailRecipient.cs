using System.ComponentModel.DataAnnotations;

namespace Azure.Comms.EmailRelay.Functions.Models;

/// <summary>
/// Represents an individual recipient for an email dispatch request.
/// </summary>
public sealed class EmailRecipient
{
    /// <summary>
    /// Email address in RFC 5322 format.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Optional display name to accompany the email address.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Role for the recipient (To, Cc, Bcc).
    /// </summary>
    public EmailRecipientType Type { get; set; } = EmailRecipientType.To;
}
