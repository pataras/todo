namespace Azure.Comms.EmailRelay.Functions.Models;

/// <summary>
/// Captures the context of an email dispatch failure for follow-up processing.
/// </summary>
public sealed class FailedEmailMessage
{
    public EmailMessage OriginalMessage { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public string? ErrorType { get; set; }

    public string? StackTrace { get; set; }

    public DateTimeOffset FailedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public int DequeueCount { get; set; }

    public IDictionary<string, string> Diagnostics { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}
