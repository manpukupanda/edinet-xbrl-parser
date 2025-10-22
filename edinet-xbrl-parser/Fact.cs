using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Represents a single XBRL fact value (numeric or non-numeric) associated with an element.
/// </summary>
public class Fact : XbrlItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Fact"/> class.
    /// </summary>
    /// <param name="dts">The discoverable taxonomy set this fact belongs to.</param>
    /// <param name="xml">The XML element representing this fact.</param>
    public Fact(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    /// <summary>
    /// Gets the element definition for this fact.
    /// </summary>
    public required Element Element { get; init; }

    /// <summary>
    /// Gets the textual value of the fact, or null if the fact is nil.
    /// </summary>
    public required string? Value { get; init; }

    /// <summary>
    /// Gets the context associated with this fact. For tuples this may be null.
    /// </summary>
    /// <remarks>Tuple facts do not have a context; non-tuple facts will have a context.</remarks>
    public required Context? Context { get; init; }

    /// <summary>
    /// Gets the unit associated with this fact, if any (for numeric facts).
    /// </summary>
    public required Unit? Unit { get; init; }

    /// <summary>
    /// Gets a value indicating whether the fact is explicitly marked as nil (xsi:nil="true").
    /// </summary>
    public required bool Nil { get; init; }

    /// <summary>
    /// Gets the value of the decimals attribute, if provided.
    /// </summary>
    public required int? Decimals { get; init; }
}

/// <summary>
/// Represents a tuple fact that contains nested member facts.
/// </summary>
public class TupleFact : Fact
{
    /// <summary>
    /// Gets the member facts contained by the tuple.
    /// </summary>
    public required Fact[] MemberFacts { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TupleFact"/> class.
    /// </summary>
    /// <param name="dts">The discoverable taxonomy set this tuple fact belongs to.</param>
    /// <param name="xml">The XML element representing this tuple fact.</param>
    public TupleFact(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }
}
