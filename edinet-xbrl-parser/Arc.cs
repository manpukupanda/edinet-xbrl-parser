using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Represents an XBRL arc (relationship) used in linkbases.
/// </summary>
public class Arc : XbrlItem
{
    /// <summary>
    /// Use attribute enum.
    /// </summary>
    public enum UseKind
    {
        /// <summary>
        /// Optional use: arc may participate in the relation network.
        /// </summary>
        optional,

        /// <summary>
        /// Prohibited use: arc prohibits inclusion of the relation in the network.
        /// </summary>
        prohibited,
    }

    internal Arc(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    /// <summary>
    /// The 'from' label of the arc.
    /// </summary>
    public required string From { get; init; }

    /// <summary>
    /// The 'to' label of the arc.
    /// </summary>
    public required string To { get; init; }

    /// <summary>
    /// The use attribute (optional/prohibited) of the arc.
    /// </summary>
    public required UseKind Use { get; init; }

    /// <summary>
    /// Priority value of the arc.
    /// </summary>
    public required int Priority { get; init; }

    /// <summary>
    /// The arcrole attribute (if any).
    /// </summary>
    public required string? Arcrole { get; init; }

    /// <summary>
    /// Order value for sorting child nodes.
    /// </summary>
    public required double Order { get; init; }

    /// <summary>
    /// Weight value for the arc (if any).
    /// </summary>
    public required int? Weight { get; init; }

    /// <summary>
    /// Preferred label value associated with this arc (if any).
    /// </summary>
    public required string? PreferredLabel { get; init; }
}
