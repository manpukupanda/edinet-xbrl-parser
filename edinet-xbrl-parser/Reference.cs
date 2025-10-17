using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

public class Reference : XBRLItem
{
    public Reference(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    public required (XName name, string value)[] Ref { get; init; }
}
