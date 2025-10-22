using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Represents a label resource used in XBRL linkbases (label link or generic link).
/// </summary>
public class Label : XbrlItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Label"/> class.
    /// </summary>
    /// <param name="dts">The discoverable taxonomy set this label belongs to.</param>
    /// <param name="xml">The XML element representing this label.</param>
    public Label(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    /// <summary>
    /// Gets the text value of the label.
    /// </summary>
    public required string Value { get; init; }

    /// <summary>
    /// Gets the optional role attribute of the label (xlink:role).
    /// </summary>
    public required string? Role { get; init; }

    /// <summary>
    /// Gets the optional language of the label (xml:lang).
    /// </summary>
    public required string? Lang { get; init; }
}
