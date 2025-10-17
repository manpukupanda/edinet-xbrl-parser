using System.Collections.Immutable;

namespace Manpuku.Edinet.Xbrl;

public class LinkTree
{
    internal class Relation
    {
        public Arc Arc { get; set; }
        public XBRLItem From { get; set; }
        public XBRLItem To { get; set; }
        public Relation(Arc arc, XBRLItem from, XBRLItem to)
        {
            Arc = arc;
            From = from;
            To = to;
        }
    }

    /// <summary>
    /// リソース(From)をキーにして、全関係を分類
    /// </summary>
    private readonly Dictionary<XBRLItem, Relation[]> _relations;

    internal LinkTree(XBRLDiscoverableTaxonomySet dts, RoleType roleType, XBRLLink.LinkKind linkKind)
    {
        DTS = dts;
        RoleType = roleType;
        LinkKind = linkKind;
        var arcRelations = ResolveArcRelation();

        var tmp = new Dictionary<XBRLItem, List<Relation>>();
        foreach (var relation in arcRelations)
        {
            tmp.TryAdd(relation.From, []);
            tmp[relation.From].Add(relation);
        }
        _relations = new Dictionary<XBRLItem, Relation[]>();
        foreach (var relation in tmp)
        {
            _relations[relation.Key] = [.. relation.Value];
        }
        var roots = GetRoots(arcRelations);
        RootNodes = [.. roots.Select(r => new Node(this, r, null))];
    }

    /// <summary>
    /// DTS
    /// </summary>
    public XBRLDiscoverableTaxonomySet DTS { get; init; }

    /// <summary>
    /// ロールタイプ
    /// </summary>
    public RoleType RoleType { get; init; }

    /// <summary>
    /// リンクの種類
    /// </summary>
    public XBRLLink.LinkKind LinkKind { get; init; }

    /// <summary>
    /// ルートノード
    /// </summary>
    public Node[] RootNodes { get; init; }

    private IEnumerable<Relation> ResolveArcRelation()
    {
        var links = LinkKind == XBRLLink.LinkKind.presentationLink ? DTS.PresentationLinks :
            LinkKind == XBRLLink.LinkKind.definitionLink ? DTS.DefinitionLinks :
            LinkKind == XBRLLink.LinkKind.calculationLink ? DTS.CalculationLinks :
            LinkKind == XBRLLink.LinkKind.labelLink ? DTS.LabelLinks :
            LinkKind == XBRLLink.LinkKind.referenceLink ? DTS.ReferenceLinks :
            LinkKind == XBRLLink.LinkKind.genericLink ? DTS.GenericLinks : 
            throw new NotImplementedException();

        var relations = new List<Relation>();
        foreach (var l in links.Where(l => l.RoleType == RoleType))
        {
            relations.AddRange(l.GetRelations());
        }
        return relations;
    }

    private IEnumerable<XBRLItem> GetRoots(IEnumerable<Relation> relations)
    {
        var fromSet = new HashSet<XBRLItem>();
        var toSet = new HashSet<XBRLItem>();
        foreach (var r in relations)
        {
            fromSet.Add(r.From);
            toSet.Add(r.To);
        }
        fromSet.ExceptWith(toSet);
        return fromSet;
    }

    /// <summary>
    /// ツリーの全ノードを深さ優先で列挙する
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Node> EnumerateDepthFirst()
    {
        foreach (var root in RootNodes)
        {
            foreach (var descendant in EnumerateDepthFirst(root))
            {
                yield return descendant;
            }
        }
    }

    private static IEnumerable<Node> EnumerateDepthFirst(Node node)
    {
        if (node == null) yield break;

        yield return node;

        foreach (var child in node.Children)
        {
            foreach (var descendant in EnumerateDepthFirst(child))
            {
                yield return descendant;
            }
        }
    }

    /// <summary>
    /// ツリーのノード
    /// </summary>
    public class Node
    {
        /// <summary>
        /// 属するツリー
        /// </summary>
        public LinkTree Tree { get; init; }

        /// <summary>
        /// リソース（要素やラベル。ロケータは設定しない。）
        /// </summary>
        public XBRLItem Resource { get; init; }

        /// <summary>
        /// 親ノード向けのエッジ
        /// </summary>
        public Edge? EdgeFromParent { get; init; }

        public double? Order => EdgeFromParent?.Arc.Order;

        public string? PreferredLabel => EdgeFromParent?.Arc.PreferredLabel;

        public string? Arcrole => EdgeFromParent?.Arc.Arcrole;

        public int? Weight => EdgeFromParent?.Arc.Weight;

        /// <summary>
        /// 親ノードを取得する
        /// </summary>
        public Node? Parent => EdgeFromParent?.Parent;

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
        /// 子ノード向けのエッジ
        /// </summary>
        private Edge[] EdgesToChildren { get; init; }

        /// <summary>
        /// 子ノードを取得する
        /// </summary>
        public IEnumerable<Node> Children => EdgesToChildren.OrderBy(e => e.Arc.Order).Select(e => e.Child);

        internal Node(LinkTree tree, XBRLItem resource, Edge? fromParent)
        {
            Tree = tree;
            Resource = resource;
            EdgeFromParent = fromParent;

            var edges = new List<Edge>();
            if (tree._relations.TryGetValue(resource, out var relations))
            {
                foreach (var group in relations.GroupBy(r => r.To))
                {
                    var child = group.Key;

                    // 最高の優先度
                    var maxPriority = group.Select(rel => rel.Arc.Priority).Max();

                    // 最高の優先度を持つ関係
                    var relationsMaxPriority = group.Where(rel => rel.Arc.Priority == maxPriority);

                    // 最高の優先度を持つ関係で禁止関係のものがあればこの関係は無効
                    if (relationsMaxPriority.Any(r => r.Arc.Use == Arc.UseKind.prohibited))
                    {
                        continue;
                    }

                    // 最高の優先度を持つ関係のうち任意の１つ
                    var relationMaxPriority = relationsMaxPriority.First();

                    // ツリーにその関係を含める
                    edges.Add(new Edge(tree, this, child, relationMaxPriority.Arc));
                }
            }
            EdgesToChildren = [.. edges];
            return;
        }
    }

    /// <summary>
    /// ノードを結ぶ枝（エッジ）
    /// </summary>
    public class Edge
    {
        internal Edge(LinkTree tree, Node parent, XBRLItem childResource, Arc arc)
        {
            Tree = tree;
            Parent = parent;
            Arc = arc;
            Child = new Node(Tree, childResource, this);
        }

        /// <summary>
        /// 属するツリー
        /// </summary>
        public LinkTree Tree { get; init; }

        /// <summary>
        /// 親ノード
        /// </summary>
        public Node Parent { get; init; }

        /// <summary>
        /// 子ノードとの関係を結ぶアーク。
        /// </summary>
        public Arc Arc { get; init; }

        /// <summary>
        /// 子ノード
        /// </summary>
        public Node Child { get; init; }
    }
}
