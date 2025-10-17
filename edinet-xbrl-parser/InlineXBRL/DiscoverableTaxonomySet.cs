using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl.InlineXBRL;

/// <summary>
/// Discoverable Taxonomy Set for Inline XBRL.
/// </summary>
public class XBRLDiscoverableTaxonomySet : Manpuku.Edinet.Xbrl.XBRLDiscoverableTaxonomySet
{
    public required (Uri Uri, XDocument Document)[] InlineXBRLs { get; init; }
}

