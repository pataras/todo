using System.ComponentModel.DataAnnotations;
using Azure.Comms.EmailRelay.Functions.Models;

namespace Azure.Comms.EmailRelay.Functions.Services;

internal static class EmailMessageValidator
{
    public static void Validate(EmailMessage message)
    {
        if (message is null)
        {
            throw new ValidationException("Email message payload cannot be null.");
        }

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(message);
        if (!Validator.TryValidateObject(message, validationContext, validationResults, validateAllProperties: true))
        {
            ThrowValidationException(validationResults);
        }

        ValidateRecipients(message.Recipients, "Recipients");
        ValidateRecipients(message.ReplyTo, "ReplyTo");
        ValidateAttachments(message.Attachments);

        if (string.IsNullOrWhiteSpace(message.Content?.PlainTextBody) &&
            string.IsNullOrWhiteSpace(message.Content?.HtmlBody) &&
            string.IsNullOrWhiteSpace(message.Content?.TemplateId))
        {
            throw new ValidationException("Email content must include plain text, HTML, or a template identifier.");
        }
    }

    private static void ValidateRecipients(IEnumerable<EmailRecipient> recipients, string propertyName)
    {
        if (recipients is null)
        {
            return;
        }

        var index = 0;
        foreach (var recipient in recipients)
        {
            index++;
            if (recipient is null)
            {
                throw new ValidationException($"{propertyName}[{index}] cannot be null.");
            }

            var results = new List<ValidationResult>();
            var context = new ValidationContext(recipient);
            if (!Validator.TryValidateObject(recipient, context, results, validateAllProperties: true))
            {
                ThrowValidationException(results, $"{propertyName}[{index}]");
            }
        }
    }

    private static void ValidateAttachments(IEnumerable<EmailAttachmentReference> attachments)
    {
        if (attachments is null)
        {
            return;
        }

        var index = 0;
        foreach (var attachment in attachments)
        {
            index++;
            if (attachment is null)
            {
                throw new ValidationException($"Attachments[{index}] cannot be null.");
            }

            var results = new List<ValidationResult>();
            var context = new ValidationContext(attachment);
            if (!Validator.TryValidateObject(attachment, context, results, validateAllProperties: true))
            {
                ThrowValidationException(results, $"Attachments[{index}]");
            }
        }
    }

    private static void ThrowValidationException(IEnumerable<ValidationResult> results, string? prefix = null)
    {
        var errors = results
            .Where(result => result != ValidationResult.Success)
            .Select(result => string.IsNullOrWhiteSpace(prefix)
                ? result.ErrorMessage
                : $"{prefix}: {result.ErrorMessage}")
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .ToArray();

        var message = errors.Length == 0
            ? "Email message validation failed."
            : string.Join("; ", errors);

        throw new ValidationException(message);
    }
}
