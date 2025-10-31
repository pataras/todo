namespace Azure.Comms.EmailRelay.Functions.Models;

/// <summary>
/// Captures operational metadata to support retries, observability, and idempotency.
/// </summary>
public sealed class EmailDispatchMetadata
{
    /// <summary>
    /// Correlates the email to upstream business operations.
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Identifier assigned by the producer to support idempotent retry logic.
    /// </summary>
    public string? ProducerMessageId { get; set; }

    /// <summary>
    /// UTC timestamp indicating when the payload was placed on the outbox queue.
    /// </summary>
    public DateTimeOffset? EnqueuedAtUtc { get; set; }

    /// <summary>
    /// Count of send attempts performed so far.
    /// </summary>
    public int AttemptCount { get; set; }

    /// <summary>
    /// Free-form bucket for downstream systems to add debugging data.
    /// </summary>
    public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
}
