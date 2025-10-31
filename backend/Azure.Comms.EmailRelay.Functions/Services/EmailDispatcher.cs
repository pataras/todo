using System.Diagnostics;
using Azure.Communication.Email;
using Azure.Comms.EmailRelay.Functions.Configuration;
using Azure.Comms.EmailRelay.Functions.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace Azure.Comms.EmailRelay.Functions.Services;

internal interface IEmailDispatcher
{
    Task<EmailSendResult> DispatchAsync(EmailMessage message, CancellationToken cancellationToken);
}

internal sealed class EmailDispatcher : IEmailDispatcher
{
    private readonly EmailClient _emailClient;
    private readonly CommunicationServiceOptions _options;
    private readonly IAttachmentLoader _attachmentLoader;
    private readonly ILogger<EmailDispatcher> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly IEmailMetricsRecorder _metrics;

    public EmailDispatcher(
        EmailClient emailClient,
        IAttachmentLoader attachmentLoader,
        IOptions<CommunicationServiceOptions> options,
        ILogger<EmailDispatcher> logger,
        IEmailMetricsRecorder metrics)
    {
        _emailClient = emailClient;
        _attachmentLoader = attachmentLoader;
        _logger = logger;
        _options = options.Value;
        _metrics = metrics;

        _retryPolicy = Policy
            .Handle<Exception>(IsTransientError)
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (exception, delay, retryAttempt, _) =>
                {
                    _logger.LogWarning(
                        exception,
                        "Transient failure when sending email. Retrying attempt {Attempt} after {Delay}.",
                        retryAttempt,
                        delay);
                });
    }

    public async Task<EmailSendResult> DispatchAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        EmailMessageValidator.Validate(message);

        var attachments = await _attachmentLoader.LoadAsync(message.Attachments, cancellationToken).ConfigureAwait(false);

        var sender = string.IsNullOrWhiteSpace(message.SenderOverride)
            ? _options.DefaultSenderAddress
            : message.SenderOverride;

        if (string.IsNullOrWhiteSpace(sender))
        {
            throw new InvalidOperationException("Email sender address is not configured.");
        }

        var toRecipients = message.Recipients
            .Where(r => r.Type == EmailRecipientType.To)
            .Select(r => new EmailAddress(r.Address) { DisplayName = r.DisplayName })
            .ToList();

        if (toRecipients.Count == 0)
        {
            throw new InvalidOperationException("At least one 'To' recipient is required.");
        }

        var ccRecipients = message.Recipients
            .Where(r => r.Type == EmailRecipientType.Cc)
            .Select(r => new EmailAddress(r.Address) { DisplayName = r.DisplayName })
            .ToList();

        var bccRecipients = message.Recipients
            .Where(r => r.Type == EmailRecipientType.Bcc)
            .Select(r => new EmailAddress(r.Address) { DisplayName = r.DisplayName })
            .ToList();

        var emailRecipients = new EmailRecipients(toRecipients)
        {
            Cc = ccRecipients,
            Bcc = bccRecipients
        };

        var emailContent = BuildContent(message);

        var emailMessage = new Azure.Communication.Email.EmailMessage(sender, emailContent, emailRecipients);

        foreach (var replyTo in message.ReplyTo ?? Array.Empty<EmailRecipient>())
        {
            emailMessage.ReplyTo.Add(new EmailAddress(replyTo.Address)
            {
                DisplayName = replyTo.DisplayName
            });
        }

        foreach (var attachment in attachments)
        {
            emailMessage.Attachments.Add(attachment);
        }

        var stopwatch = Stopwatch.StartNew();

        EmailSendResult result;
        try
        {
            result = await _retryPolicy.ExecuteAsync(ct => SendAsync(emailMessage, ct), cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _metrics.TrackDispatchFailure(message, ex, message.Metadata.AttemptCount);
            throw;
        }

        stopwatch.Stop();

        _logger.LogInformation(
            "Email {MessageId} delivered successfully to {RecipientCount} recipients in {ElapsedMs} ms. OperationId={OperationId}",
            message.MessageId,
            toRecipients.Count + ccRecipients.Count + bccRecipients.Count,
            stopwatch.Elapsed.TotalMilliseconds,
            result?.Id);

        _metrics.TrackDispatchSuccess(message, result, stopwatch.Elapsed, toRecipients.Count + ccRecipients.Count + bccRecipients.Count);

        return result!;
    }

    private EmailContent BuildContent(EmailMessage message)
    {
        if (!string.IsNullOrWhiteSpace(message.Content.TemplateId))
        {
            _logger.LogWarning(
                "TemplateId {TemplateId} specified on message {MessageId}, but template rendering is not implemented. Falling back to literal content.",
                message.Content.TemplateId,
                message.MessageId);
        }

        return new EmailContent(message.Subject)
        {
            PlainText = message.Content.PlainTextBody,
            Html = message.Content.HtmlBody
        };
    }

    private async Task<EmailSendResult> SendAsync(Azure.Communication.Email.EmailMessage emailMessage, CancellationToken cancellationToken)
    {
        var operation = await _emailClient.SendAsync(Azure.WaitUntil.Completed, emailMessage, cancellationToken).ConfigureAwait(false);
        return operation.Value;
    }

    private static bool IsTransientError(Exception exception)
    {
        return exception switch
        {
            Azure.RequestFailedException requestFailed when requestFailed.Status >= 500 => true,
            Azure.RequestFailedException requestFailed when requestFailed.Status == 429 => true,
            _ => false
        };
    }
}
