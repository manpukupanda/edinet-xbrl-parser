using System.Collections.Concurrent;
using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Represents a Discoverable Taxonomy Set (DTS) that aggregates taxonomy, instance, and linkbase information discovered from XBRL documents.
/// </summary>
public class XBRLDiscoverableTaxonomySet
{
    private Element[] _elements = [];
    private ConcurrentDictionary<string, Element>? _elementsByHref = null;
    private ConcurrentDictionary<XName, Element>? _elementsByName = null;

    /// <summary>
    /// Gets or sets the document tree structure for all documents in the DTS.
    /// </summary>
    public required DocumentTree DocumentTree { get; init; }

    /// <summary>
    /// Gets all element definitions discovered in the DTS.
    /// </summary>
    public Element[] Elements
    {
        get => _elements;
        internal set
        {
            _elements = value;
            var dic = new ConcurrentDictionary<string, Element>();
            var dic2 = new ConcurrentDictionary<XName, Element>();
            foreach (var e in value)
            {
                dic.TryAdd(e.ReferenceUri.AbsoluteUri, e);
                dic2.TryAdd(e.Name, e);
            }
            _elementsByHref = dic;
            _elementsByName = dic2;
        }
    }

    /// <summary>
    /// Gets all role types defined in the DTS.
    /// </summary>
    public RoleType[] RoleTypes { get; internal set; } = [];

    /// <summary>
    /// Gets all contexts defined in the DTS.
    /// </summary>
    public Context[] Contexts { get; internal set; } = [];

    /// <summary>
    /// Gets all units defined in the DTS.
    /// </summary>
    public Unit[] Units { get; internal set; } = [];

    /// <summary>
    /// Gets all facts (data points) defined in the DTS.
    /// </summary>
    public Fact[] Facts { get; internal set; } = [];

    /// <summary>
    /// Gets all presentation links in the DTS.
    /// </summary>
    public XBRLLink[] PresentationLinks { get; internal set; } = [];

    /// <summary>
    /// Gets the presentation link trees, grouped by role type.
    /// </summary>
    public Dictionary<RoleType, LinkTree> PresentationLinkTrees { get; internal set; } = [];

    /// <summary>
    /// Gets all definition links in the DTS.
    /// </summary>
    public XBRLLink[] DefinitionLinks { get; internal set; } = [];

    /// <summary>
    /// Gets the definition link trees, grouped by role type.
    /// </summary>
    public Dictionary<RoleType, LinkTree> DefinitionLinkTrees { get; internal set; } = [];

    /// <summary>
    /// Gets all calculation links in the DTS.
    /// </summary>
    public XBRLLink[] CalculationLinks { get; internal set; } = [];

    /// <summary>
    /// Gets the calculation link trees, grouped by role type.
    /// </summary>
    public Dictionary<RoleType, LinkTree> CalculationLinkTrees { get; internal set; } = [];

    /// <summary>
    /// Gets all label links in the DTS.
    /// </summary>
    public XBRLLink[] LabelLinks { get; internal set; } = [];

    /// <summary>
    /// Gets the label link trees, grouped by role type.
    /// </summary>
    public Dictionary<RoleType, LinkTree> LabelLinkTrees { get; internal set; } = [];

    /// <summary>
    /// Gets all reference links in the DTS.
    /// </summary>
    public XBRLLink[] ReferenceLinks { get; internal set; } = [];

    /// <summary>
    /// Gets the reference link trees, grouped by role type.
    /// </summary>
    public Dictionary<RoleType, LinkTree> ReferenceLinkTrees { get; internal set; } = [];

    /// <summary>
    /// Gets all generic links in the DTS.
    /// </summary>
    public XBRLLink[] GenericLinks { get; internal set; } = [];

    /// <summary>
    /// Gets the generic link trees, grouped by role type.
    /// </summary>
    public Dictionary<RoleType, LinkTree> GenericLinkTrees { get; internal set; } = [];

    /// <summary>
    /// Gets all labels associated with elements in the DTS.
    /// </summary>
    public Dictionary<Element, Label[]> Labels { get; internal set; } = [];

    /// <summary>
    /// Gets all references associated with elements in the DTS.
    /// </summary>
    public Dictionary<Element, Reference[]> References { get; internal set; } = [];

    /// <summary>
    /// Retrieves the element associated with the specified URI, if it exists.
    /// </summary>
    /// <param name="href">The URI used to locate the corresponding element. The method uses the absolute form of the URI for lookup.</param>
    /// <returns>The element associated with the specified URI, or null if no such element exists.</returns>
    internal Element? GetElement(Uri href)
    {
        if (_elementsByHref == null) return null;

        if (_elementsByHref.TryGetValue(href.AbsoluteUri, out var element))
        {
            return element;
        }
        return null;
    }

    /// <summary>
    /// Retrieves the element associated with the specified XML name, if it exists.
    /// </summary>
    /// <param name="name">The XML qualified name of the element to retrieve.</param>
    /// <returns>The element associated with the specified name, or null if no such element exists.</returns>
    internal Element? GetElement(XName name)
    {
        if (_elementsByName == null) return null;

        if (_elementsByName.TryGetValue(name, out var element))
        {
            return element;
        }
        return null;
    }

    /// <summary>
    /// Retrieves the URI associated with the specified XML document, if one exists.
    /// </summary>
    /// <param name="document">The XML document for which to retrieve the associated URI.</param>
    /// <returns>A <see cref="Uri"/> representing the URI associated with the specified document, or <see langword="null"/> if no
    /// association is found.</returns>
    internal Uri? GetUriFor(XDocument document)
    {
        foreach (var node in DocumentTree.Nodes)
        {
            if (node.Document == document)
            {
                return node.URI;
            }
        }
        return null;
    }

    /// <summary>
    /// Returns a collection of unique XBRL documents of the specified kind from the document tree.
    /// </summary>
    /// <remarks>Each document is included only once in the result, even if multiple nodes reference the same
    /// document URI. Documents with a null value are excluded.</remarks>
    /// <param name="documentType">The type of document to retrieve. Only documents matching this kind are included in the result.</param>
    /// <returns>An enumerable collection of XDocument instances representing the unique documents of the specified kind. The
    /// collection is empty if no matching documents are found.</returns>
    internal IEnumerable<XDocument> GetDocuments(DocumentTreeNode.DocumentKind documentType)
    {
        var seen = new HashSet<Uri>();

        foreach (var node in DocumentTree.Nodes)
        {
            if (node.NodeKind == documentType && node.Document != null && seen.Add(node.URI))
            {
                yield return node.Document;
            }
        }
    }
}
