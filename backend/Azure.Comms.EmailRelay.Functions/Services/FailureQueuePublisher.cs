using System.Text.Json;
using Azure;
using Azure.Comms.EmailRelay.Functions.Configuration;
using Azure.Comms.EmailRelay.Functions.Models;
using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Azure.Comms.EmailRelay.Functions.Services;

internal interface IFailureQueuePublisher
{
    Task PublishAsync(FailedEmailMessage message, CancellationToken cancellationToken);
}

internal sealed class FailureQueuePublisher : IFailureQueuePublisher
{
    private readonly QueueClient _queueClient;
    private readonly ILogger<FailureQueuePublisher> _logger;
    private readonly JsonSerializerOptions _serializerOptions;

    public FailureQueuePublisher(QueueServiceClient queueServiceClient, IOptions<EmailQueueOptions> options, ILogger<FailureQueuePublisher> logger)
    {
        _logger = logger;
        var queueOptions = options.Value;
        _queueClient = queueServiceClient.GetQueueClient(queueOptions.FailedQueueName);
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task PublishAsync(FailedEmailMessage message, CancellationToken cancellationToken)
    {
        await _queueClient.CreateIfNotExistsAsync(cancellationToken).ConfigureAwait(false);

        var payload = JsonSerializer.Serialize(message, _serializerOptions);
        _logger.LogError("Enqueuing failed email {MessageId} onto queue {QueueName}", message.OriginalMessage.MessageId, _queueClient.Name);

        await _queueClient.SendMessageAsync(BinaryData.FromString(payload), cancellationToken).ConfigureAwait(false);
    }
}
