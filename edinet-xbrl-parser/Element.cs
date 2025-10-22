using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Represents an XBRL element definition discovered in a taxonomy.
/// </summary>
public class Element : XbrlItem
{
    /// <summary>
    /// Specifies the period type for an element (duration, instant, or undefined).
    /// </summary>
    public enum PeriodKind
    {
        /// <summary>
        /// Period (duration)
        /// </summary>
        Duration,

        /// <summary>
        /// Instant (point in time)
        /// </summary>
        Instant,

        /// <summary>
        /// Undefined period kind
        /// </summary>
        Undefined,
    }

    /// <summary>
    /// Specifies the balance kind for an element (debit, credit, or undefined).
    /// </summary>
    public enum BalanceKind
    {
        /// <summary>
        /// Debit balance
        /// </summary>
        Debit,

        /// <summary>
        /// Credit balance
        /// </summary>
        Credit,

        /// <summary>
        /// Undefined balance kind
        /// </summary>
        Undefined,
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Element"/> class.
    /// </summary>
    /// <param name="dts">The discoverable taxonomy set this element belongs to.</param>
    /// <param name="xml">The XML element representing this XBRL element.</param>
    public Element(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    /// <summary>
    /// Gets the XNamespace of this element's name.
    /// </summary>
    public XNamespace TargetNamespace => Name.Namespace;

    /// <summary>
    /// Gets the qualified name of the element.
    /// </summary>
    public required XName Name { get; init; }

    /// <summary>
    /// Gets a value indicating whether the element is abstract.
    /// </summary>
    public required bool Abstract { get; init; }

    /// <summary>
    /// Gets the period type of the element (duration, instant, or undefined).
    /// </summary>
    public required PeriodKind PeriodType { get; init; }

    /// <summary>
    /// Gets a value indicating whether the element allows nil values.
    /// </summary>
    public required bool Nillable { get; init; }

    /// <summary>
    /// Gets the substitution group (if any) for the element.
    /// </summary>
    public required XName? SubstitutionGroup { get; init; }

    /// <summary>
    /// Gets the XBRL type (xsd type) of this element, if specified.
    /// </summary>
    public required XName? XBRLType { get; init; }

    /// <summary>
    /// Gets the balance kind (debit/credit) of the element.
    /// </summary>
    public required BalanceKind Balance { get; init; }
}

