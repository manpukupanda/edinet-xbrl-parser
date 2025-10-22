using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl.InlineXBRL;

/// <summary>
/// Represents a Discoverable Taxonomy Set (DTS) for Inline XBRL, extending the base DTS with Inline XBRL document support.
/// </summary>
public class XBRLDiscoverableTaxonomySet : Manpuku.Edinet.Xbrl.XBRLDiscoverableTaxonomySet
{
    /// <summary>
    /// Gets or sets the array of Inline XBRL document pairs (URI and XDocument).
    /// </summary>
    public required (Uri Uri, XDocument Document)[] InlineXBRLs { get; init; }
}

