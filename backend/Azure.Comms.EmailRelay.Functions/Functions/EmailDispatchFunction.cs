using Azure.Comms.EmailRelay.Functions.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Azure.Comms.EmailRelay.Functions.Functions;

/// <summary>
/// Placeholder queue-triggered function that will orchestrate email delivery in later phases.
/// </summary>
public sealed class EmailDispatchFunction
{
    private readonly ILogger<EmailDispatchFunction> _logger;

    public EmailDispatchFunction(ILogger<EmailDispatchFunction> logger)
    {
        _logger = logger;
    }

    [Function("EmailDispatchFunction")]
    public Task RunAsync(
        [QueueTrigger("%EmailQueues:OutboxQueueName%", Connection = "AzureWebJobsStorage")] EmailMessage message,
        FunctionContext context)
    {
        _logger.LogInformation(
            "EmailDispatchFunction received message {MessageId} with subject {Subject}. Implementation pending future phases.",
            message?.MessageId,
            message?.Subject);

        return Task.CompletedTask;
    }
}
