using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Represents a locator element in an XBRL linkbase, which points to a resource in a taxonomy or instance.
/// </summary>
public class Locator : XbrlItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Locator"/> class.
    /// </summary>
    /// <param name="dts">The discoverable taxonomy set this locator belongs to.</param>
    /// <param name="xml">The XML element representing this locator.</param>
    public Locator(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    /// <summary>
    /// Gets the resource referenced by this locator (an XBRL item such as an Element or RoleType).
    /// </summary>
    public required XbrlItem Resource { get; init; }

    /// <summary>
    /// Gets the href attribute value referring to the target resource (may include a fragment).
    /// </summary>
    public required string Href { get; init; }

    /// <summary>
    /// Gets the xlink:label attribute value used to associate arcs with this locator.
    /// </summary>
    public required string Label { get; init; }
}
