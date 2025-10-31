using Azure.Communication.Email;
using Azure.Comms.EmailRelay.Functions.Models;
using Microsoft.Extensions.Logging;

namespace Azure.Comms.EmailRelay.Functions.Services;

internal interface IEmailMetricsRecorder
{
    void TrackDispatchSuccess(EmailMessage message, EmailSendResult result, TimeSpan duration, int recipientCount);
    void TrackDispatchFailure(EmailMessage message, Exception exception, int dequeueCount);
    void TrackAttachmentLoadFailure(EmailAttachmentReference reference, Exception exception);
}

internal sealed class EmailMetricsRecorder : IEmailMetricsRecorder
{
    private readonly ILogger<EmailMetricsRecorder> _logger;

    public EmailMetricsRecorder(ILogger<EmailMetricsRecorder> logger)
    {
        _logger = logger;
    }

    public void TrackDispatchSuccess(EmailMessage message, EmailSendResult result, TimeSpan duration, int recipientCount)
    {
        _logger.LogInformation(
            "Metric EmailSendSuccess MessageId={MessageId} OperationId={OperationId} DurationMs={Duration} RecipientCount={RecipientCount}",
            message.MessageId,
            result.Id,
            duration.TotalMilliseconds,
            recipientCount);
    }

    public void TrackDispatchFailure(EmailMessage message, Exception exception, int dequeueCount)
    {
        _logger.LogError(
            exception,
            "Metric EmailSendFailure MessageId={MessageId} DequeueCount={DequeueCount} Subject={Subject}",
            message.MessageId,
            dequeueCount,
            message.Subject);
    }

    public void TrackAttachmentLoadFailure(EmailAttachmentReference reference, Exception exception)
    {
        _logger.LogError(
            exception,
            "Metric AttachmentLoadFailure Container={Container} Blob={Blob}",
            reference.BlobContainerName,
            reference.BlobName);
    }
}
