using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

public class XbrlParser : IXbrlParser
{
    internal readonly ILoggerFactory _loggerFactory;

    protected readonly ILogger<XBRLDiscoverableTaxonomySet> _logger;

    public static Func<Uri, Task<XDocument>> DefaultLoaderFunc = DocumentTreeLoader.LoadAsync;

    public XbrlParser(ILoggerFactory? loggerFactory)
    {
        _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        _logger = _loggerFactory.CreateLogger<XBRLDiscoverableTaxonomySet>();
    }

    public virtual async Task<XBRLDiscoverableTaxonomySet> ParseAsync(Uri entryPointUri, Func<Uri, Task<XDocument>> loader)
    {
        _logger.LogTrace("start DTS load.");
        var dts = await LoadDtsAsync(entryPointUri, loader);
        _logger.LogTrace("end DTS load.");
        _logger.LogTrace("start parse.");
        Parse(dts);
        _logger.LogTrace("end parse.");
        return dts;
    }

    protected async Task<XBRLDiscoverableTaxonomySet> LoadDtsAsync(Uri entryPointUri, Func<Uri, Task<XDocument>> loader)
    {
        var documentTreeLoader = new DocumentTreeLoader(_loggerFactory, loader);
        var tree = await documentTreeLoader.CreateAsync(entryPointUri, null);
        var dts = new XBRLDiscoverableTaxonomySet() { DocumentTree = tree };
        return dts;
    }

    private protected virtual (XbrlSchemaParser SchemaParser, XbrlInstanceParser InstanceParser, XbrlLinkbaseParser LinkbaseParser) CreateParsers(XBRLDiscoverableTaxonomySet dts)
    {
        return (new XbrlSchemaParser(dts, _loggerFactory),
                new XbrlInstanceParser(dts, _loggerFactory),
                new XbrlLinkbaseParser(dts, _loggerFactory));
    }

    protected void Parse(XBRLDiscoverableTaxonomySet dts)
    {
        var (SchemaParser, InstanceParser, LinkbaseParser) = CreateParsers(dts);

        SchemaParser.Parse();
        _logger.LogTrace("ParseSchema OK.");

        InstanceParser.Parse();
        _logger.LogTrace("ParseInstance OK.");

        LinkbaseParser.Parse();
        _logger.LogTrace("ParseLinkbase OK.");

        ResolveLabels(dts);
        _logger.LogTrace("ResolveLabels OK.");

        ResolveReferences(dts);
        _logger.LogTrace("ResolveReferences OK.");

        ResolveGenericLinks(dts);
        _logger.LogTrace("ResolveGenericLinks OK.");
    }

    void ResolveLabels(XBRLDiscoverableTaxonomySet dts)
    {
        var linkRt = dts.RoleTypes.FirstOrDefault(rt => rt.RoleURI == XbrlNamespaces.DefaultLinkRole);
        if (linkRt != null && dts.LabelLinkTrees.ContainsKey(linkRt))
        {
            Dictionary<Element, List<Label>> tmp = new();
            var labelTree = dts.LabelLinkTrees[linkRt];
            foreach (var node in labelTree.RootNodes)
            {
                if (node.Resource is Element element)
                {
                    foreach (var child in node.Children)
                    {
                        if (child.Resource is Label label)
                        {
                            tmp.TryAdd(element, []);
                            tmp[element].Add(label);
                        }
                    }
                }
            }
            dts.Labels = tmp.ToDictionary(k => k.Key, k => k.Value.ToArray());
        }
    }

    void ResolveReferences(XBRLDiscoverableTaxonomySet dts)
    {
        var linkRt = dts.RoleTypes.FirstOrDefault(rt => rt.RoleURI == XbrlNamespaces.DefaultLinkRole);
        if (linkRt != null && dts.ReferenceLinkTrees.ContainsKey(linkRt))
        {
            Dictionary<Element, List<Reference>> tmp = new();
            var refTree = dts.ReferenceLinkTrees[linkRt];
            foreach (var node in refTree.RootNodes)
            {
                if (node.Resource is Element element)
                {
                    foreach (var child in node.Children)
                    {
                        if (child.Resource is Reference reference)
                        {
                            tmp.TryAdd(element, []);
                            tmp[element].Add(reference);
                        }
                    }
                }
            }
            dts.References = tmp.ToDictionary(k => k.Key, k => k.Value.ToArray());
        }
    }

    void ResolveGenericLinks(XBRLDiscoverableTaxonomySet dts)
    {
        foreach (var genericLinkTree in dts.GenericLinkTrees.Values)
        {
            foreach (var node in genericLinkTree.RootNodes)
            {
                foreach (var child in node.Children)
                {
                    if (child.Resource is Label label && node.Resource is RoleType rt)
                    {
                        rt.DefinitionEn = label.Value;
                    }
                }
            }
        }
    }
}
