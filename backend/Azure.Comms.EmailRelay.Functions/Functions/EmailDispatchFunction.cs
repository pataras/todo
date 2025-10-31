using System.ComponentModel.DataAnnotations;
using Azure.Comms.EmailRelay.Functions.Configuration;
using Azure.Comms.EmailRelay.Functions.Models;
using Azure.Comms.EmailRelay.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Azure.Comms.EmailRelay.Functions.Functions;

public sealed class EmailDispatchFunction
{
    private readonly ILogger<EmailDispatchFunction> _logger;
    private readonly IEmailDispatcher _emailDispatcher;
    private readonly IFailureQueuePublisher _failureQueuePublisher;
    private readonly EmailQueueOptions _queueOptions;

    public EmailDispatchFunction(
        ILogger<EmailDispatchFunction> logger,
        IEmailDispatcher emailDispatcher,
        IFailureQueuePublisher failureQueuePublisher,
        IOptions<EmailQueueOptions> queueOptions)
    {
        _logger = logger;
        _emailDispatcher = emailDispatcher;
        _failureQueuePublisher = failureQueuePublisher;
        _queueOptions = queueOptions.Value;
    }

    [Function("EmailDispatchFunction")]
    public async Task RunAsync(
        [QueueTrigger("%EmailQueues:OutboxQueueName%", Connection = "AzureWebJobsStorage")] EmailMessage message,
        FunctionContext context)
    {
        var cancellationToken = context.CancellationToken;
        var dequeueCount = GetDequeueCount(context);

        if (message.Metadata is not null)
        {
            message.Metadata.AttemptCount = dequeueCount;
        }

        try
        {
            await _emailDispatcher.DispatchAsync(message, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            var fatal = IsFatal(ex);

            _logger.LogError(
                ex,
                "Failed to dispatch email {MessageId} on attempt {Attempt}. Fatal={Fatal}",
                message?.MessageId,
                dequeueCount,
                fatal);

            var shouldMoveToFailedQueue = fatal || dequeueCount >= _queueOptions.MaxDequeueCount;

            if (!shouldMoveToFailedQueue)
            {
                throw;
            }

            var failedMessage = new FailedEmailMessage
            {
                OriginalMessage = message,
                ErrorMessage = ex.Message,
                ErrorType = ex.GetType().FullName,
                StackTrace = ex.ToString(),
                FailedAtUtc = DateTimeOffset.UtcNow,
                DequeueCount = dequeueCount,
                Diagnostics = new Dictionary<string, string>
                {
                    ["FunctionInvocationId"] = context.InvocationId,
                    ["HostInstanceId"] = context.InstanceId,
                    ["DequeueCount"] = dequeueCount.ToString(),
                    ["IsFatal"] = fatal.ToString()
                }
            };

            await _failureQueuePublisher.PublishAsync(failedMessage, cancellationToken).ConfigureAwait(false);

            _logger.LogWarning(
                "Email {MessageId} moved to failed queue after {Attempts} attempts.",
                message?.MessageId,
                dequeueCount);
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

    private static bool IsFatal(Exception exception)
    {
        return exception switch
        {
            ValidationException => true,
            InvalidOperationException => true,
            _ => false
        };
    }
}
