using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

public class Locator : XBRLItem
{
    public Locator(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    public required XBRLItem Resource { get; init; }

    public required string Href { get; init; }

    public required string Label { get; init; }
}
