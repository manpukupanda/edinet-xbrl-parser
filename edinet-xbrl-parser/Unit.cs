using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Represents a unit definition in an XBRL instance or taxonomy.
/// </summary>
public class Unit : XbrlItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Unit"/> class.
    /// </summary>
    /// <param name="dts">The discoverable taxonomy set this unit belongs to.</param>
    /// <param name="xml">The XML element representing this unit.</param>
    public Unit(DiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }
}
