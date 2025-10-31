using System.Text.Json;
using Azure.Comms.EmailRelay.Functions.Configuration;
using Azure.Comms.EmailRelay.Functions.Models;
using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Azure.Comms.EmailRelay.Functions.Services;

internal interface IOutboxQueuePublisher
{
    Task PublishAsync(EmailMessage message, TimeSpan? visibilityDelay, CancellationToken cancellationToken);
}

internal sealed class OutboxQueuePublisher : IOutboxQueuePublisher
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<OutboxQueuePublisher> _logger;
    private readonly JsonSerializerOptions _serializerOptions;

    public OutboxQueuePublisher(QueueServiceClient queueServiceClient, IOptions<EmailQueueOptions> options, ILogger<OutboxQueuePublisher> logger)
    {
        _logger = logger;
        var queueOptions = options.Value;
        _queueClient = queueServiceClient.GetQueueClient(queueOptions.OutboxQueueName);
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task PublishAsync(EmailMessage message, TimeSpan? visibilityDelay, CancellationToken cancellationToken)
    {
        await _queueClient.CreateIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

        var payload = JsonSerializer.Serialize(message, _serializerOptions);
        _logger.LogInformation("Resubmitting email {MessageId} to queue {QueueName}", message.MessageId, _queueClient.Name);

        await _queueClient.SendMessageAsync(
            messageText: payload,
            visibilityTimeout: visibilityDelay,
            cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
