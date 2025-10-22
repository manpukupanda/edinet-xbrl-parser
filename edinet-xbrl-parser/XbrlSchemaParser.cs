using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using static Manpuku.Edinet.Xbrl.Element;

namespace Manpuku.Edinet.Xbrl;

internal class XbrlSchemaParser
{
    readonly ILoggerFactory _loggerFactory;

    readonly ILogger _logger;

    /// <summary>
    /// The discoverable taxonomy set (DTS) that this parser populates with elements and role types.
    /// </summary>
    public XBRLDiscoverableTaxonomySet Dts { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="XbrlSchemaParser"/> class.
    /// </summary>
    /// <param name="dts">The discoverable taxonomy set to populate.</param>
    /// <param name="loggerFactory">Logger factory used to create loggers for diagnostics.</param>
    public XbrlSchemaParser(XBRLDiscoverableTaxonomySet dts, ILoggerFactory loggerFactory)
    {
        Dts = dts;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<XbrlSchemaParser>();
    }

    /// <summary>
    /// Parse taxonomy schema documents in the DTS and populate the DTS with discovered elements and role types.
    /// </summary>
    public void Parse()
    {
        Dts.Elements = [.. ParseElement()];
        _logger.LogTrace("ParseElement OK.");

        Dts.RoleTypes = [.. ParseRoleType()];
        _logger.LogTrace("ParseRoleType OK.");
    }

    IEnumerable<Element> ParseElement()
    {
        var result = new List<Element>();

        var documents = Dts.GetDocuments(DocumentTreeNode.DocumentKind.TaxonomySchema);

        foreach (var doc in documents)
        {
            var root = doc.Root;
            if (root is XElement elementRoot)
            {
                foreach (var xml in elementRoot.Elements(XbrlNamespaces.xsdElement))
                {
                    var element = CreateElement(xml);
                    result.Add(element);
                }
            }
        }

        return result;
    }

    Element CreateElement(XElement xml)
    {
        XNamespace targetNamespace = xml.Document?.Root?.AttributeString(XbrlNamespaces.targetNamespace) ?? throw new InvalidDataException();

        var _name = xml.AttributeString(XbrlNamespaces.name) ?? throw new InvalidDataException();
        var name = targetNamespace + _name;

        var _abstract = xml.AttributeString(XbrlNamespaces.@abstract);
        var @abstract = bool.TryParse(_abstract, out bool value) ? value : false;
        var _periodType = xml.AttributeString(XbrlNamespaces.xbrliPeriodType);
        var periodType = Enum.TryParse<PeriodKind>(_periodType, true, out PeriodKind b) ? b : PeriodKind.Undefined;
        var _nillable = xml.AttributeString(XbrlNamespaces.nillable);
        var nillable = bool.TryParse(_nillable, out bool _nil) ? !_nil : false;
        var _substitutionGroup = xml.AttributeString(XbrlNamespaces.substitutionGroup);
        var substitutionGroup = _substitutionGroup == null ? null : xml.ResolveQName(_substitutionGroup);
        var _type = xml.AttributeString(XbrlNamespaces.type);
        var xBRLType = _type == null ? null : xml.ResolveQName(_type);
        var _balance = xml.AttributeString(XbrlNamespaces.xbrliBalance);
        var balance = Enum.TryParse<BalanceKind>(_balance, true, out BalanceKind bal) ? bal : BalanceKind.Undefined;

        var isTuple = xml.ResolveQName(xml.AttributeString(XbrlNamespaces.substitutionGroup)) == XbrlNamespaces.xbrliTuple;
        if (isTuple)
        {
            var seq = xml.Elements(XbrlNamespaces.xsdComplexType)
                         .Elements(XbrlNamespaces.xsdSequence)
                         .Elements(XbrlNamespaces.xsdElement)
                         .Select(e => ParseMember(e));

            return
                new Tuple(Dts, xml)
                {
                    Name = name,
                    Abstract = @abstract,
                    PeriodType = periodType,
                    Nillable = nillable,
                    SubstitutionGroup = substitutionGroup,
                    XBRLType = xBRLType,
                    Balance = balance,
                    Sequence = [.. seq],
                };
        }
        else
        {
            return
                new Element(Dts, xml)
                {
                    Name = name,
                    Abstract = @abstract,
                    PeriodType = periodType,
                    Nillable = nillable,
                    SubstitutionGroup = substitutionGroup,
                    XBRLType = xBRLType,
                    Balance = balance,
                };
        }
    }

    Tuple.Member ParseMember(XElement xml)
    {
        var maxOccurs = xml.AttributeInt(XbrlNamespaces.maxOccurs);
        var minOccurs = xml.AttributeInt(XbrlNamespaces.minOccurs);
        var elementRefStr = xml.AttributeString(XbrlNamespaces.@ref);
        if (elementRefStr == null)
        {
            var (code, message) = XbrlErrorCatalog.MissingAttribute("ref");
            throw new XbrlSyntaxException(code, message);
        }
        var elementRef = xml.ResolveQName(elementRefStr);
        if (elementRef == null)
        {
            var (code, message) = XbrlErrorCatalog.UnresolvedQName(elementRefStr);
            throw new XbrlSyntaxException(code, message);
        }
        return new Tuple.Member(Dts, xml)
        {
            MaxOccurs = maxOccurs,
            MinOccurs = minOccurs,
            ElementRef = elementRef,
        };
    }

    IEnumerable<RoleType> ParseRoleType()
    {
        var result = new List<RoleType>();

        foreach (var document in Dts.GetDocuments(DocumentTreeNode.DocumentKind.TaxonomySchema))
        {
            var root = document.Root;
            if (root is not XElement) continue;

            foreach (var annotation in root.Elements(XbrlNamespaces.xsdAnnotation))
            {
                foreach (var appinfo in annotation.Elements(XbrlNamespaces.xsdAppinfo))
                {
                    foreach (var roleType in appinfo.Elements(XbrlNamespaces.linkRoleType))
                    {
                        var role = new RoleType(Dts, roleType)
                        {
                            Definition = ParseDefinition(roleType),
                            RoleURI = ParseRoleURI(roleType),
                            UsedOns = [.. ParseUsedOns(roleType)],
                        };
                        result.Add(role);
                    }
                }
            }
        }

        result.Add(new RoleType(Dts, XbrlItem.DummyElement)
        {
            Definition = null,
            RoleURI = XbrlNamespaces.DefaultLinkRole,
            UsedOns = [],
        });
        result.Add(new RoleType(Dts, XbrlItem.DummyElement)
        {
            Definition = null,
            RoleURI = XbrlNamespaces.DefaultLinkRole2008,
            UsedOns = [],
        });

        return result;
    }

    static string? ParseDefinition(XElement xml)
    {
        var defi = xml.Element(XbrlNamespaces.linkDefinition);
        if (defi != null)
            return defi.Value;
        else
            return null;
    }

    static string ParseRoleURI(XElement xml)
    {
        var role = xml.AttributeString(XbrlNamespaces.roleURI);
        if (role == null)
        {
            var (code, message) = XbrlErrorCatalog.MissingAttribute("roleURI");
            throw new XbrlSyntaxException(code, message);
        }
        return role;
    }

    static IEnumerable<XName> ParseUsedOns(XElement xml)
    {
        var result = new List<XName>();
        foreach (var usedOn in xml.Descendants(XbrlNamespaces.linkUsedOn))
        {
            var value = usedOn.Value;
            if (value == null)
            {
                var (code, message) = XbrlErrorCatalog.EmptyValue(XbrlNamespaces.linkUsedOn.LocalName);
                throw new XbrlSyntaxException(code, message);
            }
            var name = usedOn.ResolveQName(value);
            if (name == null)
            {
                var (code, message) = XbrlErrorCatalog.UnresolvedQName(value);
                throw new XbrlSyntaxException(code, message);
            }
            result.Add(name);
        }
        return result;
    }
}
