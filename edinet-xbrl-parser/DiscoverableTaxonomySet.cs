using System.Collections.Concurrent;
using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Discoverable Taxonomy Set
/// </summary>
public class XBRLDiscoverableTaxonomySet
{
    private Element[] _elements = [];
    private ConcurrentDictionary<string, Element>? _elementsByHref = null;
    private ConcurrentDictionary<XName, Element>? _elementsByName = null;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    internal XBRLDiscoverableTaxonomySet() { }

    /// <summary>
    /// ドキュメント構成のルートドキュメント
    /// </summary>
    public required XBRLDocumentTree DocumentTree { get; init; }

    /// <summary>
    /// 全要素
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
    /// 全ロールタイプ
    /// </summary>
    public RoleType[] RoleTypes { get; internal set; } = [];

    /// <summary>
    /// 全コンテキスト
    /// </summary>
    public Context[] Contexts { get; internal set; } = [];

    /// <summary>
    /// 全ユニット
    /// </summary>
    public Unit[] Units { get; internal set; } = [];

    /// <summary>
    /// 全ファクト
    /// </summary>
    public Fact[] Facts { get; internal set; } = [];

    /// <summary>
    /// 全表示リンク
    /// </summary>
    public XBRLLink[] PresentationLinks { get; internal set; } = [];
    public Dictionary<RoleType, LinkTree> PresentationLinkTrees { get; internal set; } = [];

    /// <summary>
    /// 全定義リンク
    /// </summary>
    public XBRLLink[] DefinitionLinks { get; internal set; } = [];
    public Dictionary<RoleType, LinkTree> DefinitionLinkTrees { get; internal set; } = [];

    /// <summary>
    /// 計算リンク
    /// </summary>
    public XBRLLink[] CalculationLinks { get; internal set; } = [];
    public Dictionary<RoleType, LinkTree> CalculationLinkTrees { get; internal set; } = [];

    /// <summary>
    /// ラベルリンク
    /// </summary>
    public XBRLLink[] LabelLinks { get; internal set; } = [];
    public Dictionary<RoleType, LinkTree> LabelLinkTrees { get; internal set; } = [];

    /// <summary>
    /// 参照リンク
    /// </summary>
    public XBRLLink[] ReferenceLinks { get; internal set; } = [];
    public Dictionary<RoleType, LinkTree> ReferenceLinkTrees { get; internal set; } = [];

    /// <summary>
    /// ラベルリンク
    /// </summary>
    public XBRLLink[] GenericLinks { get; internal set; } = [];
    public Dictionary<RoleType, LinkTree> GenericLinkTrees { get; internal set; } = [];

    /// <summary>
    /// 全ラベル
    /// </summary>
    public Dictionary<Element, Label[]> Labels { get; internal set; } = [];

    /// <summary>
    /// 全リファレンス
    /// </summary>
    public Dictionary<Element, Reference[]> References { get; internal set; } = [];

    /// <summary>
    /// 要素を取得する
    /// </summary>
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
    /// 要素を取得する
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
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
    /// ドキュメントからURIを取得する
    /// </summary>
    /// <param name="document"></param>
    /// <returns></returns>
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
    /// 特定の種類のドキュメントを返す
    /// </summary>
    /// <param name="documentType"></param>
    /// <returns></returns>
    internal IEnumerable<XDocument> GetDocuments(XBRLDocumentTreeNode.DocumentKind documentType)
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

