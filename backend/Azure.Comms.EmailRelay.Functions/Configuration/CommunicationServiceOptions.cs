using System.ComponentModel.DataAnnotations;

namespace Azure.Comms.EmailRelay.Functions.Configuration;

/// <summary>
/// Configuration that governs the Azure Communication Services email client.
/// </summary>
public sealed class CommunicationServiceOptions
{
    public const string SectionName = "CommunicationService";

    [Required]
    public string ConnectionString { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string DefaultSenderAddress { get; set; } = string.Empty;

    /// <summary>
    /// Optional default reply-to address for email dispatch.
    /// </summary>
    [EmailAddress]
    public string? DefaultReplyToAddress { get; set; }
}
