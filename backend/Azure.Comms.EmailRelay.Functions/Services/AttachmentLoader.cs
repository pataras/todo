using System.IO;
using Azure.Communication.Email;
using Azure.Comms.EmailRelay.Functions.Configuration;
using Azure.Comms.EmailRelay.Functions.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Azure.Comms.EmailRelay.Functions.Services;

internal interface IAttachmentLoader
{
    Task<IReadOnlyList<EmailAttachment>> LoadAsync(IEnumerable<EmailAttachmentReference> references, CancellationToken cancellationToken);
}

internal sealed class AttachmentLoader : IAttachmentLoader
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobStorageOptions _options;
    private readonly ILogger<AttachmentLoader> _logger;
    private readonly IEmailMetricsRecorder _metrics;

    public AttachmentLoader(
        BlobServiceClient blobServiceClient,
        IOptions<BlobStorageOptions> options,
        ILogger<AttachmentLoader> logger,
        IEmailMetricsRecorder metrics)
    {
        _blobServiceClient = blobServiceClient;
        _options = options.Value;
        _logger = logger;
        _metrics = metrics;
    }

    public async Task<IReadOnlyList<EmailAttachment>> LoadAsync(IEnumerable<EmailAttachmentReference> references, CancellationToken cancellationToken)
    {
        if (references is null)
        {
            return Array.Empty<EmailAttachment>();
        }

        var attachments = new List<EmailAttachment>();

        foreach (var reference in references)
        {
            if (reference is null)
            {
                continue;
            }

            var containerName = string.IsNullOrWhiteSpace(reference.BlobContainerName)
                ? _options.ContainerName
                : reference.BlobContainerName;

            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(reference.BlobName);

            _logger.LogInformation("Fetching attachment {BlobName} from container {Container}", reference.BlobName, containerName);

            BlobDownloadResult downloadResult;
            try
            {
                downloadResult = await blobClient.DownloadContentAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download attachment {BlobName} from container {Container}", reference.BlobName, containerName);
                _metrics.TrackAttachmentLoadFailure(reference, ex);
                throw;
            }

            var fileName = string.IsNullOrWhiteSpace(reference.FileName)
                ? Path.GetFileName(reference.BlobName)
                : reference.FileName;

            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = reference.BlobName;
            }

            var contentType = reference.ContentType;
            if (string.IsNullOrWhiteSpace(contentType))
            {
                contentType = downloadResult.Details.ContentType;
            }

            if (string.IsNullOrWhiteSpace(contentType))
            {
                contentType = "application/octet-stream";
            }

            attachments.Add(new EmailAttachment(fileName, contentType, downloadResult.Content));
        }

        return attachments;
    }
}
