using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

public class Label : XBRLItem
{
    public Label(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    public required string Value { get; init; }

    public required string? Role { get; init; }

    public required string? Lang { get; init; }
}
