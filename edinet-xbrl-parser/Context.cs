using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

public class Context : XBRLItem
{
    public class ExplicitMember
    {
        public required Element Dimension { get; init; }
        public required Element Member { get; init; }
    }

    public Context(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    public required string StartDate { get; init; }
    public required string EndDate { get; init; }
    public required string Instant { get; init; }
    public required ExplicitMember[] Scenario { get; init; }
}
