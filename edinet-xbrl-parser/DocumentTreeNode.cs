using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Represents a node in the document tree. A node corresponds to a loaded XML document and keeps
/// references to related documents (schema refs, imports, linkbase refs, locs) as child nodes.
/// </summary>
public class DocumentTreeNode
{
    /// <summary>
    /// Kind of document represented by the node.
    /// </summary>
    public enum DocumentKind
    {
        /// <summary>
        /// Other / unknown document type.
        /// </summary>
        Other,

        /// <summary>
        /// Taxonomy schema (XSD schema).
        /// </summary>
        TaxonomySchema,

        /// <summary>
        /// XBRL instance document (xbrli:xbrl).
        /// </summary>
        Instance,

        /// <summary>
        /// Linkbase document (link:linkbase).
        /// </summary>
        Linkbase,
    }

    /// <summary>
    /// Determine document kind from the XDocument root element.
    /// </summary>
    /// <param name="doc">XDocument to inspect.</param>
    /// <returns>Detected document kind.</returns>
    internal static DocumentKind KindOf(XDocument doc)
    {
        if (doc.Root?.Name == XbrlNamespaces.xbrliXbrl)
            return DocumentKind.Instance;
        else if (doc.Root?.Name == XbrlNamespaces.xsdSchema)
            return DocumentKind.TaxonomySchema;
        else if (doc.Root?.Name == XbrlNamespaces.linkLinkbase)
            return DocumentKind.Linkbase;
        else
            return DocumentKind.Other;
    }

    /// <summary>
    /// Internal constructor. Initializes URI, document, and parent.
    /// </summary>
    internal DocumentTreeNode(Uri uri, XDocument document, DocumentTreeNode? parent)
    {
        URI = uri;
        Document = document;
        Parent = parent;
        NodeKind = KindOf(Document);
    }

    /// <summary>
    /// The URI of the document represented by this node.
    /// </summary>
    public Uri URI { get; private set; }

    /// <summary>
    /// The loaded XDocument for this node.
    /// </summary>
    public XDocument Document { get; private set; }

    /// <summary>
    /// Parent node (null for root).
    /// </summary>
    public DocumentTreeNode? Parent { get; private set; }

    /// <summary>
    /// Distance (depth) from the root. Root is0.
    /// </summary>
    public int Distance
    {
        get
        {
            if (Parent == null) return 0;
            return 1 + Parent.Distance;
        }
    }

    /// <summary>
    /// Enumerates ancestor nodes from parent to the top.
    /// </summary>
    public IEnumerable<DocumentTreeNode> Ancestors
    {
        get
        {
            for (var n = Parent; n != null; n = n.Parent)
            {
                yield return n;
            }
        }
    }

    /// <summary>
    /// The kind of document stored in this node.
    /// </summary>
    public DocumentKind NodeKind { get; private set; }

    /// <summary>
    /// Nodes referenced via schemaRef / xsd:import etc. Empty array when unset.
    /// </summary>
    public DocumentTreeNode[] SchemaRefs { get; internal set; } = Array.Empty<DocumentTreeNode>();

    /// <summary>
    /// Imported schema nodes.
    /// </summary>
    public DocumentTreeNode[] Imports { get; internal set; } = Array.Empty<DocumentTreeNode>();

    /// <summary>
    /// Linkbase reference nodes.
    /// </summary>
    public DocumentTreeNode[] LinkbaseRefs { get; internal set; } = Array.Empty<DocumentTreeNode>();

    /// <summary>
    /// Nodes corresponding to link:loc entries.
    /// </summary>
    public DocumentTreeNode[] Locs { get; internal set; } = Array.Empty<DocumentTreeNode>();

    /// <summary>
    /// Returns direct child nodes combined from SchemaRefs, Imports, LinkbaseRefs and Locs in that order.
    /// </summary>
    public DocumentTreeNode[] Children => SchemaRefs
    .Concat(Imports)
    .Concat(LinkbaseRefs)
    .Concat(Locs)
    .ToArray();
}
