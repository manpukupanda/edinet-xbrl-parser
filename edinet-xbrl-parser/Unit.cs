using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

public class Unit : XBRLItem
{
    public Unit(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }
}
