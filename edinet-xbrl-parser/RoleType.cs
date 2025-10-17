using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

public class RoleType : XBRLItem
{
    public RoleType(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    /// <summary>
    /// definition
    /// </summary>
    public required string? Definition { get; init; }

    /// <summary>
    /// RuleURI
    /// </summary>
    public required string RoleURI { get; init; }

    public required XName[] UsedOns { get; init; }

    public string? DefinitionEn { get; internal set; }
}