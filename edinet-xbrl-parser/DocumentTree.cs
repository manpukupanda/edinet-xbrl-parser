using System.Net;
using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

public class XBRLDocumentTree
{
    internal XBRLDocumentTree(XBRLDocumentTreeNode root)
    {
        Root = root;
    }

    public XBRLDocumentTreeNode Root { get; private set; }

    public IEnumerable<XBRLDocumentTreeNode> Nodes => TraverseDfs(Root);

    private static IEnumerable<XBRLDocumentTreeNode> TraverseDfs(XBRLDocumentTreeNode node)
    {
        yield return node;
        foreach (var child in node.Children)
        {
            foreach (var desc in TraverseDfs(child))
            {
                yield return desc;
            }
        }
    }
}

/// <summary>
/// ドキュメント構成をツリーで表すときに、ノードを示す
/// </summary>
public class XBRLDocumentTreeNode
{
    /// <summary>
    /// ドキュメントの種類
    /// </summary>
    public enum DocumentKind
    {
        /// <summary>
        /// その他
        /// </summary>
        Other,

        /// <summary>
        /// タクソノミ
        /// </summary>
        TaxonomySchema,

        /// <summary>
        /// インスタンス
        /// </summary>
        Instance,

        /// <summary>
        /// リンクベース
        /// </summary>
        Linkbase,
    }

    /// <summary>
    /// ドキュメントの種類を判別する
    /// </summary>
    /// <param name="doc"></param>
    /// <returns></returns>
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

    internal XBRLDocumentTreeNode(Uri uri, XDocument document, XBRLDocumentTreeNode? parent)
    {
        URI = uri;
        Document = document;
        Parent = parent;
        NodeKind = KindOf(Document);
    }

    /// <summary>
    /// ドキュメントのURI
    /// </summary>
    public Uri URI { get; private set; }

    /// <summary>
    /// ドキュメント
    /// </summary>
    public XDocument Document { get; private set; }

    /// <summary>
    /// ドキュメントの親ノード
    /// </summary>
    public XBRLDocumentTreeNode? Parent { get; private set; }

    /// <summary>
    /// ルートからの距離
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
    /// 祖先を取得する
    /// </summary>
    public IEnumerable<XBRLDocumentTreeNode> Ancestors
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
    /// ドキュメントの種別を取得する
    /// </summary>
    public DocumentKind NodeKind { get; private set; }

    public XBRLDocumentTreeNode[] SchemaRefs { get; internal set; } = [];

    public XBRLDocumentTreeNode[] Imports { get; internal set; } = [];

    public XBRLDocumentTreeNode[] LinkbaseRefs { get; internal set; } = [];

    public XBRLDocumentTreeNode[] Locs { get; internal set; } = [];

    public XBRLDocumentTreeNode[] Children => [.. SchemaRefs.Concat(Imports).Concat(LinkbaseRefs).Concat(Locs)];
}

