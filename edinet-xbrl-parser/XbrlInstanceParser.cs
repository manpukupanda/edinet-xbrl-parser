using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

internal class XbrlInstanceParser
{
    readonly ILoggerFactory _loggerFactory;

    readonly ILogger _logger;

    public DiscoverableTaxonomySet Dts { get; init; }

    public XbrlInstanceParser(DiscoverableTaxonomySet dts, ILoggerFactory loggerFactory)
    {
        Dts = dts;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<XbrlInstanceParser>();
    }

    public virtual void Parse()
    {
        Dts.Contexts = [.. ParseContext()];
        _logger.LogTrace("ParseContext OK.");

        Dts.Units = [.. ParseUnit()];
        _logger.LogTrace("ParseUnit OK.");

        Dts.Facts = [.. ParseFact()];
        _logger.LogTrace("ParseFact OK.");
    }

    protected virtual IEnumerable<Context> ParseContext()
    {
        var result = new List<Context>();
        var documents = Dts.GetDocuments(DocumentTreeNode.DocumentKind.Instance);

        foreach (var document in documents)
        {
            var contextElements = document.Descendants(XbrlNamespaces.xbrliContext);
            foreach (var c in contextElements)
            {
                var context = new Context(Dts, c)
                {
                    StartDate = ParseDate(c, XbrlNamespaces.xbrliStartDate),
                    EndDate = ParseDate(c, XbrlNamespaces.xbrliEndDate),
                    Instant = ParseDate(c, XbrlNamespaces.xbrliInstant),
                    Scenario = ParseScenario(c),
                };
                result.Add(context);
            }
        }

        return result;
    }

    protected static string ParseDate(XElement xml, XName name)
    {
        return xml.Descendants(name).FirstOrDefault()?.Value ?? string.Empty;
    }

    protected Context.ExplicitMember[] ParseScenario(XElement xml)
    {
        var scenario = xml.Descendants(XbrlNamespaces.xbrliScenario).FirstOrDefault();
        if (scenario != null)
        {
            return [.. ParseExplicitMember(scenario)];
        }
        return [];
    }

    protected Context.ExplicitMember[] ParseExplicitMember(XElement scenario)
    {
        var tmp = new List<Context.ExplicitMember>();
        foreach (var explicitMember in scenario.Descendants(XbrlNamespaces.xbrldiExplicitMember))
        {
            var dimension = explicitMember.AttributeString(XbrlNamespaces.dimension);
            if (dimension == null) continue;

            var value = explicitMember.Value;

            var dimensionName = explicitMember.ResolveQName(dimension);
            if (dimensionName == null)
            {
                var (code, message) = XbrlErrorCatalog.UnresolvedQName(dimension);
                throw new XbrlSyntaxException(code, message);
            }

            var valueName = explicitMember.ResolveQName(value);
            if (valueName == null)
            {
                var (code, message) = XbrlErrorCatalog.UnresolvedQName(value);
                throw new XbrlSyntaxException(code, message);
            }

            var dimensionElement = Dts.GetConcept(dimensionName);
            if (dimensionElement == null)
            {
                var (code, message) = XbrlErrorCatalog.ElementNotFound(dimensionName.ToString());
                throw new XbrlSemanticException(code, message);
            }

            var memberElement = Dts.GetConcept(valueName);
            if (memberElement == null)
            {
                var (code, message) = XbrlErrorCatalog.ElementNotFound(valueName.ToString());
                throw new XbrlSemanticException(code, message);
            }

            tmp.Add(new Context.ExplicitMember() { Dimension = dimensionElement, Member = memberElement });
        }
        return [.. tmp];
    }

    protected virtual IEnumerable<Unit> ParseUnit()
    {
        var result = new List<Unit>();

        var documents = Dts.GetDocuments(DocumentTreeNode.DocumentKind.Instance);
        foreach (var document in documents)
        {
            var elements = document.Descendants(XbrlNamespaces.xbrliUnit);
            foreach (var xml in elements)
            {
                var unit = new Unit(Dts, xml);
                result.Add(unit);
            }
        }

        return result;
    }

    protected virtual IEnumerable<Fact> ParseFact()
    {
        var result = new List<Fact>();

        var documents = Dts.GetDocuments(DocumentTreeNode.DocumentKind.Instance);
        foreach (var doc in documents)
        {
            var root = doc.Root;
            if (root is XElement elementRoot)
            {
                foreach (var e in elementRoot.Elements())
                {
                    var ns = e.Name.NamespaceName;
                    if (ns != XbrlNamespaces.xbrli && ns != XbrlNamespaces.link)
                    {
                        var fact = CreateFact(e);
                        result.Add(fact);
                    }
                }
            }
        }

        return result;
    }

    protected Fact CreateFact(XElement xml)
    {
        var concept = Dts.GetConcept(xml.Name);
        if (concept == null)
        {
            var (code, message) = XbrlErrorCatalog.ElementNotFound(xml.Name.ToString());
            throw new XbrlSemanticException(code, message);
        }
        var value = xml.Value;
        var nil = ParseNil(xml);
        var unit = GetUnit(xml);
        var decimals = ParseDecimals(xml);
        if (xml.HasElements)
        {
            var member = xml.Elements().Select(e => CreateFact(e));
            return new TupleFact(Dts, xml)
            {
                Concept = concept,
                Value = value,
                Nil = nil,
                Context = null,
                Unit = unit,
                Decimals = decimals,
                MemberFacts = [.. member],
            };
        }
        else
        {
            var context = GetContext(xml);
            return new Fact(Dts, xml)
            {
                Concept = concept,
                Value = value,
                Nil = nil,
                Context = context,
                Unit = unit,
                Decimals = decimals,
            };
        }
    }

    protected static bool ParseNil(XElement xml)
    {
        return xml.AttributeBool(XbrlNamespaces.xsiNil) ?? false;
    }

    protected Context GetContext(XElement xml)
    {
        var contextRef = xml.AttributeString(XbrlNamespaces.contextRef);
        if (contextRef == null)
        {
            var (code, message) = XbrlErrorCatalog.MissingAttribute("contextRef");
            throw new XbrlSyntaxException(code, message);
        }

        var context = Dts.Contexts.FirstOrDefault(c => c.Id == contextRef);
        if (context == null)
        {
            var (code, message) = XbrlErrorCatalog.ContextNotFound(contextRef);
            throw new XbrlSemanticException(code, message);
        }
        return context;
    }

    protected Unit? GetUnit(XElement xml)
    {
        return Dts.Units.FirstOrDefault(u => u.Id == xml.AttributeString(XbrlNamespaces.unitRef));
    }

    protected static int? ParseDecimals(XElement xml)
    {
        return xml.AttributeInt(XbrlNamespaces.decimals);
    }
}
