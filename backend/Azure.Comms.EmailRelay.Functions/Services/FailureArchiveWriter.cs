using System.Text.Json;
using Azure;
using Azure.Comms.EmailRelay.Functions.Configuration;
using Azure.Comms.EmailRelay.Functions.Models;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Azure.Comms.EmailRelay.Functions.Services;

internal interface IFailureArchiveWriter
{
    Task WriteAsync(FailedEmailMessage message, CancellationToken cancellationToken);
}

internal sealed class FailureArchiveWriter : IFailureArchiveWriter
{
    private readonly BlobContainerClient _containerClient;
    private readonly ILogger<FailureArchiveWriter> _logger;
    private readonly JsonSerializerOptions _serializerOptions;

    public FailureArchiveWriter(BlobServiceClient blobServiceClient, IOptions<BlobStorageOptions> options, ILogger<FailureArchiveWriter> logger)
    {
        _logger = logger;
        var storageOptions = options.Value;
        _containerClient = blobServiceClient.GetBlobContainerClient(storageOptions.LogsContainerName);
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    public async Task WriteAsync(FailedEmailMessage message, CancellationToken cancellationToken)
    {
        await _containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken).ConfigureAwait(false);

        var blobName = $"failures/{message.FailedAtUtc:yyyy/MM/dd}/{message.OriginalMessage.MessageId}-{Guid.NewGuid():N}.json";
        var blobClient = _containerClient.GetBlobClient(blobName);

        var payload = JsonSerializer.Serialize(message, _serializerOptions);
        await blobClient.UploadAsync(BinaryData.FromString(payload), overwrite: true, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Archived failed email {MessageId} to blob {BlobName}", message.OriginalMessage.MessageId, blobName);
    }
}
