using System.ComponentModel.DataAnnotations;

namespace Azure.Comms.EmailRelay.Functions.Configuration;

/// <summary>
/// Strongly-typed options controlling the storage queues that drive email processing.
/// </summary>
public sealed class EmailQueueOptions
{
    public const string SectionName = "EmailQueues";

    [Required]
    public string OutboxQueueName { get; set; } = "email-outbox";

    [Required]
    public string FailedQueueName { get; set; } = "email-failed";

    /// <summary>
    /// Maximum number of dequeue attempts before the message is considered poison and moved to the failed queue.
    /// </summary>
    [Range(1, 20)]
    public int MaxDequeueCount { get; set; } = 5;

    /// <summary>
    /// Visibility timeout in seconds to apply for manual rescheduling logic if needed.
    /// </summary>
    [Range(30, 3600)]
    public int VisibilityTimeoutSeconds { get; set; } = 300;
}
