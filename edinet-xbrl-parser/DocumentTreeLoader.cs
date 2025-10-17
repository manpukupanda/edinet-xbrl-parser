using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using static Manpuku.Edinet.Xbrl.XBRLDocumentTreeNode;

namespace Manpuku.Edinet.Xbrl;

internal class DocumentTreeLoader
{
    readonly ILoggerFactory _loggerFactory;
    readonly ILogger _logger;
    readonly Func<Uri, Task<XDocument>> _loadAsync;

    public DocumentTreeLoader(ILoggerFactory loggerFactory, Func<Uri, Task<XDocument>> loadAsync)
    {
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<DocumentTreeLoader>();
        _loadAsync = loadAsync;
    }

    public async Task<XBRLDocumentTree> CreateAsync(Uri uri, XBRLDocumentTreeNode? parent)
    {
        var root = await CreateNodeAsync(uri, parent);
        return new XBRLDocumentTree(root);
    }

    async Task<XBRLDocumentTreeNode> CreateNodeAsync(Uri uri, XBRLDocumentTreeNode? parent)
    {
        _logger.LogTrace("Load: {uri}", uri);
        var document = await _loadAsync(uri);
        if (document.Root == null)
        {
            throw new InvalidDataException($"Document root is null. {uri}");
        }

        var node = new XBRLDocumentTreeNode(uri, document, parent);
        if (node.NodeKind == DocumentKind.Instance)
        {
            node.SchemaRefs = await CreateChildrenAsync(
                node,
                document.Root.Elements(XbrlNamespaces.linkSchemaRef),
                XbrlNamespaces.xlinkHref
            );
        }
        else if (node.NodeKind == DocumentKind.TaxonomySchema)
        {
            node.Imports = await CreateChildrenAsync(
                node,
                document.Root.Elements(XbrlNamespaces.xsdImport),
                XbrlNamespaces.schemaLocation
            );

            node.LinkbaseRefs = await CreateChildrenAsync(
                node,
                document.Root
                    .Elements(XbrlNamespaces.xsdAnnotation)
                    .Elements(XbrlNamespaces.xsdAppinfo)
                    .Elements(XbrlNamespaces.linkLinkbaseRef),
                XbrlNamespaces.xlinkHref
            );

        }
        else if (node.NodeKind == DocumentKind.Linkbase)
        {
            var locs = document.Root.Descendants(XbrlNamespaces.linkLoc);

            var hrefs = locs
                .Select(loc => new { loc, href = loc.AttributeString(XbrlNamespaces.xlinkHref) })
                .Where(x => x.href != null)
                .Select(x => new { x.loc, href = x.href!.Split('#')[0] })
                .DistinctBy(x => x.href)
                .ToList();

            var tasks = hrefs.Select(async x =>
            {
                var n = await CreateChildAsync(node, new Uri(uri, x.href));
                return n;
            });

            var results = await Task.WhenAll(tasks);
            node.Locs = [.. results.OfType<XBRLDocumentTreeNode>()];
        }

        return node;
    }

    async Task<XBRLDocumentTreeNode[]> CreateChildrenAsync(XBRLDocumentTreeNode node, IEnumerable<XElement> elements, XName hrefOrLocation)
    {
        var tasks = elements.Select(e => CreateChildAsync(node, e, hrefOrLocation)).ToArray();

        try
        {
            var results = await Task.WhenAll(tasks);
            return [.. results.OfType<XBRLDocumentTreeNode>()];
        }
        catch (Exception ex)
        {
            // どれか1つでも例外があれば、それを投げる（最初の1つ）
            throw ex.InnerException ?? ex;
        }
    }

    async Task<XBRLDocumentTreeNode?> CreateChildAsync(XBRLDocumentTreeNode node, XElement xml, XName name)
    {
        if (xml.Attribute(name) == null) return null;

        var u = new Uri(node.URI, xml.AttributeString(name));
        return await CreateChildAsync(node, u);
    }

    async Task<XBRLDocumentTreeNode?> CreateChildAsync(XBRLDocumentTreeNode node, Uri u)
    {
        if (Regex.IsMatch(u.AbsoluteUri, @"https?://(www\.)?xbrl\.org/"))
        {
            return null;
        }
        if (node.Ancestors.Any(a => a.URI.AbsoluteUri == u.AbsoluteUri))
        {
            return null;
        }
        return await CreateNodeAsync(u, node);
    }

    internal static async Task<XDocument> LoadAsync(Uri uri)
    {
        if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
        {
            using var client = new HttpClient();
            var stream = await client.GetStreamAsync(uri);
            return await XDocument.LoadAsync(stream, LoadOptions.None, default);
        }
        else if (uri.Scheme == Uri.UriSchemeFile)
        {
            using var stream = File.OpenRead(uri.LocalPath);
            return await XDocument.LoadAsync(stream, LoadOptions.None, default);
        }
        else
        {
            throw new NotSupportedException($"Unsupported URI scheme: {uri.Scheme}");
        }
    }
}
