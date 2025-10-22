using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Represents a reference resource contained in a reference link.
/// Each reference contains a sequence of name/value pairs describing the reference.
/// </summary>
public class Reference : XbrlItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Reference"/> class.
    /// </summary>
    /// <param name="dts">The discoverable taxonomy set this reference belongs to.</param>
    /// <param name="xml">The XML element representing this reference.</param>
    public Reference(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    /// <summary>
    /// Gets the collection of name/value pairs that make up the reference content (e.g. source, publisher, etc.).
    /// </summary>
    public required (XName name, string value)[] Ref { get; init; }
}
