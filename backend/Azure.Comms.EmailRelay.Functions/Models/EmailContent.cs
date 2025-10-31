using System.ComponentModel.DataAnnotations;

namespace Azure.Comms.EmailRelay.Functions.Models;

/// <summary>
/// Describes the textual or templated content of an email message.
/// </summary>
public sealed class EmailContent
{
    /// <summary>
    /// Plain-text fallback body. Optional when using HTML or templates.
    /// </summary>
    public string? PlainTextBody { get; set; }

    /// <summary>
    /// HTML body. Optional when using templates.
    /// </summary>
    public string? HtmlBody { get; set; }

    /// <summary>
    /// Optional template identifier understood by downstream processors.
    /// </summary>
    public string? TemplateId { get; set; }

    /// <summary>
    /// Template context expressed as a set of key/value pairs.
    /// </summary>
    public IDictionary<string, string> TemplateParameters { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Indicates whether the HTML body should be interpreted as the primary content.
    /// </summary>
    public bool PreferHtml { get; set; }
}
