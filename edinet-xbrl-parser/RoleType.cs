using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Represents a roleType resource (definition of a link role) in an XBRL taxonomy.
/// </summary>
public class RoleType : XbrlItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoleType"/> class.
    /// </summary>
    /// <param name="dts">The discoverable taxonomy set this role type belongs to.</param>
    /// <param name="xml">The XML element representing this role type.</param>
    public RoleType(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    /// <summary>
    /// Gets the human-readable definition text (may be localized).
    /// </summary>
    public required string? Definition { get; init; }

    /// <summary>
    /// Gets the role URI identifying this role type.
    /// </summary>
    public required string RoleURI { get; init; }

    /// <summary>
    /// Gets the list of usedOn values for this role type.
    /// </summary>
    public required XName[] UsedOns { get; init; }

    /// <summary>
    /// Gets the English definition assigned from generic/label links when resolved.
    /// </summary>
    public string? DefinitionEn { get; internal set; }
}