using Azure.Comms.EmailRelay.Functions.Configuration;
using Azure.Comms.EmailRelay.Functions.Models;
using Azure.Comms.EmailRelay.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Azure.Comms.EmailRelay.Functions.Functions;

public sealed class FailedEmailMonitorFunction
{
    private readonly ILogger<FailedEmailMonitorFunction> _logger;
    private readonly IFailureArchiveWriter _failureArchiveWriter;
    private readonly IOutboxQueuePublisher _outboxQueuePublisher;
    private readonly EmailQueueOptions _queueOptions;

    public FailedEmailMonitorFunction(
        ILogger<FailedEmailMonitorFunction> logger,
        IFailureArchiveWriter failureArchiveWriter,
        IOutboxQueuePublisher outboxQueuePublisher,
        IOptions<EmailQueueOptions> queueOptions)
    {
        _logger = logger;
        _failureArchiveWriter = failureArchiveWriter;
        _outboxQueuePublisher = outboxQueuePublisher;
        _queueOptions = queueOptions.Value;
    }

    [Function("FailedEmailMonitorFunction")]
    public async Task RunAsync(
        [QueueTrigger("%EmailQueues:FailedQueueName%", Connection = "AzureWebJobsStorage")] FailedEmailMessage failedMessage,
        FunctionContext context)
    {
        var cancellationToken = context.CancellationToken;
        var dequeueCount = GetDequeueCount(context);

        _logger.LogWarning(
            "Processing failed email {MessageId} from failed queue. DequeueCount={DequeueCount}",
            failedMessage?.OriginalMessage?.MessageId,
            dequeueCount);

        await _failureArchiveWriter.WriteAsync(failedMessage, cancellationToken).ConfigureAwait(false);

        if (failedMessage?.Diagnostics is not null &&
            failedMessage.Diagnostics.TryGetValue("AutoRetry", out var autoRetryValue) &&
            bool.TryParse(autoRetryValue, out var autoRetry) &&
            autoRetry)
        {
            var visibilityDelay = TimeSpan.FromSeconds(Math.Clamp(_queueOptions.VisibilityTimeoutSeconds, 30, 3600));
            failedMessage.OriginalMessage.Metadata.AttemptCount = 0;

            await _outboxQueuePublisher.PublishAsync(failedMessage.OriginalMessage, visibilityDelay, cancellationToken).ConfigureAwait(false);

            _logger.LogWarning(
                "Automatically resubmitted failed email {MessageId} to outbox queue after archival.",
                failedMessage.OriginalMessage.MessageId);
        }
        else
        {
            _logger.LogInformation(
                "Failed email {MessageId} archived. Manual intervention required.",
                failedMessage?.OriginalMessage?.MessageId);
        }
    }

    private static int GetDequeueCount(FunctionContext context)
    {
        if (context.BindingContext.BindingData.TryGetValue("DequeueCount", out var value))
        {
            if (value is int intValue)
            {
                return intValue;
            }

            if (value is string strValue && int.TryParse(strValue, out var parsed))
            {
                return parsed;
            }
        }

        return 1;
    }
}
