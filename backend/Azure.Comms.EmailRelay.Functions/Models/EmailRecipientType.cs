namespace Azure.Comms.EmailRelay.Functions.Models;

/// <summary>
/// Classifies the role of a recipient on an email message.
/// </summary>
public enum EmailRecipientType
{
    To = 0,
    Cc = 1,
    Bcc = 2
}
