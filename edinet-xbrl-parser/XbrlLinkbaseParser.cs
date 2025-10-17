using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static Manpuku.Edinet.Xbrl.Arc;

namespace Manpuku.Edinet.Xbrl;

internal class XbrlLinkbaseParser
{

    readonly ILoggerFactory _loggerFactory;

    readonly ILogger _logger;

    public XbrlLinkbaseParser(XBRLDiscoverableTaxonomySet dts, ILoggerFactory loggerFactory)
    {
        Dts = dts;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<XbrlLinkbaseParser>();
    }

    public XBRLDiscoverableTaxonomySet Dts { get; init; }

    public void Parse()
    {
        Dts.PresentationLinks = [.. ParseLink(XBRLLink.LinkKind.presentationLink)];
        Dts.PresentationLinkTrees = ParseLinkTrees(XBRLLink.LinkKind.presentationLink, Dts.PresentationLinks);
        _logger.LogInformation("PresentationLinks Parse OK.");

        Dts.DefinitionLinks = [.. ParseLink(XBRLLink.LinkKind.definitionLink)];
        Dts.DefinitionLinkTrees = ParseLinkTrees(XBRLLink.LinkKind.definitionLink, Dts.DefinitionLinks);
        _logger.LogInformation("DefinitionLinks Parse OK.");

        Dts.CalculationLinks = [.. ParseLink(XBRLLink.LinkKind.calculationLink)];
        Dts.CalculationLinkTrees = ParseLinkTrees(XBRLLink.LinkKind.calculationLink, Dts.CalculationLinks);
        _logger.LogInformation("CalculationLinks Parse OK.");

        Dts.LabelLinks = [.. ParseLink(XBRLLink.LinkKind.labelLink)];
        Dts.LabelLinkTrees = ParseLinkTrees(XBRLLink.LinkKind.labelLink, Dts.LabelLinks);
        _logger.LogInformation("LabelLinks Parse OK.");

        Dts.ReferenceLinks = [.. ParseLink(XBRLLink.LinkKind.referenceLink)];
        Dts.ReferenceLinkTrees = ParseLinkTrees(XBRLLink.LinkKind.referenceLink, Dts.ReferenceLinks);
        _logger.LogInformation("ReferenceLinks Parse OK.");

        Dts.GenericLinks = [.. ParseLink(XBRLLink.LinkKind.genericLink)];
        Dts.GenericLinkTrees = ParseLinkTrees(XBRLLink.LinkKind.genericLink, Dts.GenericLinks);
        _logger.LogInformation("GenericLinks Parse OK.");
    }

    IEnumerable<XBRLLink> ParseLink(XBRLLink.LinkKind kind)
    {
        var result = new List<XBRLLink>();

        foreach (var linkbaseNode in Dts.DocumentTree.Nodes.Where(n => n.NodeKind == XBRLDocumentTreeNode.DocumentKind.Linkbase))
        {
            var elements = linkbaseNode.Document.Descendants(XBRLLink.TagNameOf(kind));
            foreach (var element in elements)
            {
                var link = new XBRLLink(Dts, element)
                {
                    Kind = kind,
                    RoleType = GetRoleType(element),
                    Locators = [.. ParseLocator(linkbaseNode.URI, element)],
                    Labels = [.. ParseLabel(element)],
                    References = [.. ParseReference(element)],
                    Arcs = [.. ParseArc(element)],
                };
                result.Add(link);
            }
        }
        return result;
    }

    Dictionary<RoleType, LinkTree> ParseLinkTrees(XBRLLink.LinkKind linkKind, XBRLLink[] links)
    {
        var result = new Dictionary<RoleType, LinkTree>();
        foreach (var roleType in links.Select(l => l.RoleType).Distinct())
        {
            var linkTree = new LinkTree(Dts, roleType, linkKind);
            result[roleType] = linkTree;
        }
        return result;
    }

    RoleType GetRoleType(XElement xml)
    {
        var role = xml.AttributeString(XbrlNamespaces.xlinkRole);
        if (role == null)
        {
            var (code, message) = XbrlErrorCatalog.MissingAttribute("role");
            throw new XbrlSyntaxException(code, message);
        }

        var rt = Dts.RoleTypes.FirstOrDefault(r => r.RoleURI == role);
        if (rt == null)
        {
            var (code, message) = XbrlErrorCatalog.RoleTypeNotFound(role);
            throw new XbrlSyntaxException(code, message);
        }

        return rt;
    }

    IEnumerable<Locator> ParseLocator(Uri uri, XElement xml)
    {
        foreach (var loc in xml.Elements(XbrlNamespaces.linkLoc))
        {
            var href = loc.AttributeString(XbrlNamespaces.xlinkHref);
            if (href == null)
            {
                var (code, message) = XbrlErrorCatalog.MissingAttribute("href");
                throw new XbrlSyntaxException(code, message);
            }
            var label = loc.AttributeString(XbrlNamespaces.xlinkLabel);
            if (label == null)
            {
                var (code, message) = XbrlErrorCatalog.MissingAttribute("label");
                throw new XbrlSyntaxException(code, message);
            }

            yield return new Locator(Dts, loc)
            {
                Label = label,
                Href = href,
                Resource = GetResource(uri, href),
            };
        }
    }

    XBRLItem GetResource(Uri uri, string href)
    {
        var match = Regex.Match(href, @"^(.*)#(.*)$");
        if (match.Success)
        {
            Uri _uri;
            if (uri.IsFile)
            {
                var builder = new UriBuilder(new Uri(uri, match.Groups[1].Value))
                {
                    Fragment = match.Groups[2].Value
                };
                _uri = builder.Uri;
            }
            else
            {
                _uri = new Uri(uri, href);
            }
            XBRLItem? e = Dts.GetElement(_uri) ?? (XBRLItem?)Dts.RoleTypes.FirstOrDefault(r => r.ReferenceUri.AbsoluteUri == _uri.AbsoluteUri);
            if (e == null)
            {
                var (code, message) = XbrlErrorCatalog.ElementNotFound(_uri.AbsoluteUri);
                throw new XbrlSyntaxException(code, message);
            }
            return e;
        }
        else
        {
            var (code, message) = XbrlErrorCatalog.ElementNotFound(href);
            throw new XbrlSyntaxException(code, message);
        }
    }

    IEnumerable<Label> ParseLabel(XElement xml)
    {
        var result = new List<Label>();

        var labelNames = new XName[] { XbrlNamespaces.linkLabel, XbrlNamespaces.labelLabel };
        foreach (var labelName in labelNames)
        {
            foreach (var e in xml.Elements(labelName))
            {
                var label = new Label(Dts, e)
                {
                    Value = e.Value,
                    Role = e.AttributeString(XbrlNamespaces.xlinkRole),
                    Lang = e.AttributeString(XbrlNamespaces.xmlLang),
                };

                result.Add(label);
            }
        }
        return result;
    }

    IEnumerable<Reference> ParseReference(XElement xml)
    {
        var result = new List<Reference>();
        foreach (var e in xml.Elements(XbrlNamespaces.linkReference))
        {
            var refs = e.Elements().Select(_e => (name: _e.Name, value: _e.Value)).ToArray();
            var reference = new Reference(Dts, e) { Ref = refs };
            result.Add(reference);
        }
        return result;
    }

    IEnumerable<Arc> ParseArc(XElement xml)
    {
        foreach (var a in xml.Elements().Where(e => e.AttributeString(XbrlNamespaces.xlinkType) == "arc"))
        {
            var from = a.AttributeString(XbrlNamespaces.xlinkFrom);
            if (from == null)
            {
                var (code, message) = XbrlErrorCatalog.MissingAttribute("from");
                throw new XbrlSyntaxException(code, message);
            }
            var to = a.AttributeString(XbrlNamespaces.xlinkTo);
            if (to == null)
            {
                var (code, message) = XbrlErrorCatalog.MissingAttribute("to");
                throw new XbrlSyntaxException(code, message);
            }

            var use = a.AttributeString(XbrlNamespaces.use) == "prohibited"
                ? UseKind.prohibited
                : UseKind.optional;

            var priority = a.AttributeInt(XbrlNamespaces.priority) ?? 0;
            var arcrole = a.AttributeString(XbrlNamespaces.xlinkArcrole);
            var order = a.AttributeDouble(XbrlNamespaces.order) ?? 1.0;
            var weight = a.AttributeInt(XbrlNamespaces.weight);
            var preferredLabel = a.AttributeString(XbrlNamespaces.preferredLabel);

            yield return new Arc(Dts, a)
            {
                From = from,
                To = to,
                Use = use,
                Priority = priority,
                Arcrole = arcrole,
                Order = order,
                Weight = weight,
                PreferredLabel = preferredLabel,
            };
        }
    }
}
