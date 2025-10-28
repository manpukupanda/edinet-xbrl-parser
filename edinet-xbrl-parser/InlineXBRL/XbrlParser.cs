using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl.InlineXBRL;

/// <summary>
/// Inline XBRL parser for processing Inline XBRL documents and producing a discoverable taxonomy set (DTS).
/// </summary>
public class XbrlParser : Xbrl.XbrlParser, IXbrlParser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XbrlParser"/> class for Inline XBRL.
    /// </summary>
    /// <param name="loggerFactory">Logger factory for diagnostics.</param>
    public XbrlParser(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }

    /// <summary>
    /// Not supported for Inline XBRL. Throws an exception if called.
    /// </summary>
    /// <param name="entryPointUri">Entry point URI (not used).</param>
    /// <param name="loader">Loader function (not used).</param>
    /// <returns>Never returns; always throws.</returns>
    /// <exception cref="InvalidOperationException">Always thrown. Use <see cref="ParseInlineAsync"/> instead.</exception>
    public sealed override async Task<Xbrl.DiscoverableTaxonomySet> ParseAsync(Uri entryPointUri, Func<Uri, Task<XDocument>> loader)
    {
        throw new InvalidOperationException("Use ParseInline instead of Parse for Inline XBRL documents.");
    }

    /// <summary>
    /// Parses the specified Inline XBRL documents and returns a discoverable taxonomy set (DTS).
    /// </summary>
    /// <param name="inlineXBRLsURI">URIs of all Inline XBRL files to parse.</param>
    /// <param name="loader">Function to load an XDocument from a URI.</param>
    /// <returns>The populated <see cref="DiscoverableTaxonomySet"/>.</returns>
    public async Task<DiscoverableTaxonomySet> ParseInlineAsync(Uri[] inlineXBRLsURI, Func<Uri, Task<XDocument>> loader)
    {
        _logger.LogTrace("start DTS load.");
        var dts = await LoadDtsAsync(inlineXBRLsURI, loader);
        _logger.LogTrace("end DTS load.");
        _logger.LogTrace("start parse.");
        Parse(dts);
        _logger.LogTrace("end parse.");
        return dts;
    }

    /// <summary>
    /// Asynchronously loads an XBRL Discoverable Taxonomy Set (DTS) from a collection of inline XBRL document URIs
    /// using the specified document loader.
    /// </summary>
    /// <param name="inlineXBRLsURI">An array of URIs referencing the inline XBRL documents to be loaded. Each URI should point to a valid inline
    /// XBRL file.</param>
    /// <param name="loader">A function that asynchronously loads an XDocument from a given URI. This function is invoked for each URI in the
    /// collection.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an XBRLDiscoverableTaxonomySet
    /// constructed from the loaded inline XBRL documents and their referenced schema.</returns>
    /// <exception cref="InvalidDataException">Thrown if any loaded XBRL document does not have a valid root element.</exception>
    /// <exception cref="XbrlSemanticException">Thrown if no schemaRef element is found in any of the provided inline XBRL documents.</exception>
    protected async Task<DiscoverableTaxonomySet> LoadDtsAsync(Uri[] inlineXBRLsURI, Func<Uri, Task<XDocument>> loader)
    {
        // 非同期で XDocument を取得
        var loadTasks = inlineXBRLsURI.Select(async uri =>
        {
            var doc = await loader(uri);
            if (doc.Root == null)
            {
                throw new InvalidDataException($"Document root is null. {uri}");
            }
            return new { uri, doc };
        });

        var ixbrlDocs = await Task.WhenAll(loadTasks);

        // schemaRef の URI を抽出
        Uri? schema = null;

        foreach (var x in ixbrlDocs)
        {
            var references = x.doc.Descendants(XbrlNamespaces.ixReferences);
            foreach (var refElement in references)
            {
                var schemaRefs = refElement.Elements(Xbrl.XbrlNamespaces.linkSchemaRef);
                foreach (var schemaRef in schemaRefs)
                {
                    var href = schemaRef.AttributeString(Xbrl.XbrlNamespaces.xlinkHref);
                    if (!string.IsNullOrEmpty(href))
                    {
                        schema = new Uri(x.uri, href);
                        break;
                    }
                }
                if (schema != null) break;
            }
            if (schema != null) break;
        }

        if (schema == null)
        {
            var (code, message) = XbrlErrorCatalog.SchemaRefNotFound();
            throw new XbrlSemanticException(message, code);
        }

        var documentTreeLoader = new DocumentTreeLoader(_loggerFactory, loader);
        var tree = await documentTreeLoader.CreateAsync(schema, null);
        var dts = new DiscoverableTaxonomySet()
        {
            DocumentTree = tree,
            InlineXBRLs = [.. ixbrlDocs.Select(x => (x.uri, x.doc))],
        };

        return dts;
    }

    private protected override (XbrlSchemaParser SchemaParser, Xbrl.XbrlInstanceParser InstanceParser, XbrlLinkbaseParser LinkbaseParser) CreateParsers(Xbrl.DiscoverableTaxonomySet dts)
    {
        return (new XbrlSchemaParser(dts, _loggerFactory),
                new XbrlInstanceParser((DiscoverableTaxonomySet)dts, _loggerFactory),
                new XbrlLinkbaseParser(dts, _loggerFactory));
    }
}