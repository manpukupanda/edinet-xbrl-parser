using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Top-level XBRL parser coordinating DTS loading and parsing of schema, instance and linkbase documents.
/// Use <see cref="ParseAsync(Uri, Func{Uri, Task{XDocument}})"/> to parse an entry point and produce a
/// <see cref="XBRLDiscoverableTaxonomySet"/>.
/// </summary>
public class XbrlParser : IXbrlParser
{
    internal readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Provides logging capabilities for the XBRLDiscoverableTaxonomySet class.
    /// </summary>
    /// <remarks>Intended for use within the class and its derived types to record diagnostic and operational
    /// information. The logger instance is initialized externally and should not be modified after
    /// construction.</remarks>
    protected readonly ILogger<XBRLDiscoverableTaxonomySet> _logger;

    /// <summary>
    /// Default loader function used to fetch an <see cref="XDocument"/> for a given <see cref="Uri"/>.
    /// This can be overridden by supplying a custom loader to <see cref="ParseAsync(Uri, Func{Uri, Task{XDocument}})"/>.
    /// </summary>
    public static Func<Uri, Task<XDocument>> DefaultLoaderFunc = DocumentTreeLoader.LoadAsync;

    /// <summary>
    /// Initializes a new instance of the <see cref="XbrlParser"/> class.
    /// </summary>
    /// <param name="loggerFactory">Optional logger factory for diagnostic logging. If null, a <see cref="NullLoggerFactory"/> is used.</param>
    public XbrlParser(ILoggerFactory? loggerFactory)
    {
        _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        _logger = _loggerFactory.CreateLogger<XBRLDiscoverableTaxonomySet>();
    }

    /// <summary>
    /// Parse XBRL documents starting from the specified entry point URI and using the provided loader to fetch documents.
    /// The resulting <see cref="XBRLDiscoverableTaxonomySet"/> contains discovered elements, contexts, units, facts and links.
    /// </summary>
    /// <param name="entryPointUri">URI of the initial XBRL entry document (instance or schema).</param>
    /// <param name="loader">Function that loads an <see cref="XDocument"/> for a given <see cref="Uri"/>.</param>
    /// <returns>A task that represents the asynchronous parse operation. The task result is the populated <see cref="XBRLDiscoverableTaxonomySet"/>.</returns>
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

    /// <summary>
    /// Asynchronously loads an XBRL Discoverable Taxonomy Set (DTS) from the specified entry point URI using the
    /// provided document loader.
    /// </summary>
    /// <param name="entryPointUri">The URI of the entry point document for the taxonomy set. This must be a valid, absolute URI identifying the
    /// root of the DTS to load.</param>
    /// <param name="loader">A delegate that asynchronously loads an XML document given its URI. The function should return a task that
    /// produces the loaded XDocument.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the loaded
    /// XBRLDiscoverableTaxonomySet instance.</returns>
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

    /// <summary>
    /// Parses the specified XBRL discoverable taxonomy set, including its schema, instance, and linkbase components,
    /// and resolves associated labels, references, and generic links.
    /// </summary>
    /// <remarks>This method processes the provided taxonomy set by invoking specialized parsers for each XBRL
    /// component and then resolves additional relationships such as labels, references, and generic links. The method
    /// is intended to be called as part of a larger XBRL processing workflow and assumes that the input taxonomy set is
    /// valid and fully loaded.</remarks>
    /// <param name="dts">The discoverable taxonomy set to parse. Must not be null.</param>
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
