namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Represents a tree structure of XBRL linkbase relationships for a specific role type and link kind.
/// </summary>
/// <remarks>A LinkTree models the hierarchical relationships defined in an XBRL linkbase, such as presentation,
/// definition, or calculation links, for a given role type. It provides access to the root nodes of the tree and allows
/// traversal of the entire structure. This class is typically used to analyze or process the relationships between XBRL
/// taxonomy elements as defined by a particular linkbase and role. The tree structure reflects the directed
/// relationships between XBRL items, enabling depth-first enumeration and access to node and edge metadata.</remarks>
public class LinkTree
{
    internal class Relation
    {
        public Arc Arc { get; set; }
        public XbrlItem From { get; set; }
        public XbrlItem To { get; set; }
        public Relation(Arc arc, XbrlItem from, XbrlItem to)
        {
            Arc = arc;
            From = from;
            To = to;
        }
    }

    /// <summary>
    /// Represents a tree structure of XBRL linkbase relationships for a specific role type and link kind.
    /// </summary>
    private readonly Dictionary<XbrlItem, Relation[]> _relations;

    internal LinkTree(DiscoverableTaxonomySet dts, RoleType roleType, XBRLLink.LinkKind linkKind)
    {
        DTS = dts;
        RoleType = roleType;
        LinkKind = linkKind;
        var arcRelations = ResolveArcRelation();

        var tmp = new Dictionary<XbrlItem, List<Relation>>();
        foreach (var relation in arcRelations)
        {
            tmp.TryAdd(relation.From, []);
            tmp[relation.From].Add(relation);
        }
        _relations = new Dictionary<XbrlItem, Relation[]>();
        foreach (var relation in tmp)
        {
            _relations[relation.Key] = [.. relation.Value];
        }
        var roots = GetRoots(arcRelations);
        RootNodes = [.. roots.Select(r => new Node(this, r, null))];
    }

    /// <summary>
    /// Gets the discoverable taxonomy set (DTS) associated with this link tree.
    /// </summary>
    public DiscoverableTaxonomySet DTS { get; init; }

    /// <summary>
    /// Gets the role type associated with this link tree.
    /// </summary>
    public RoleType RoleType { get; init; }

    /// <summary>
    /// Gets the kind of link (presentation, definition, calculation, etc.) for this tree.
    /// </summary>
    public XBRLLink.LinkKind LinkKind { get; init; }

    /// <summary>
    /// Gets the root nodes of the link tree.
    /// </summary>
    public Node[] RootNodes { get; init; }

    private IEnumerable<Relation> ResolveArcRelation()
    {
        var links = 
            LinkKind == XBRLLink.LinkKind.presentationLink ? DTS.PresentationLinks :
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

    private IEnumerable<XbrlItem> GetRoots(IEnumerable<Relation> relations)
    {
        var fromSet = new HashSet<XbrlItem>();
        var toSet = new HashSet<XbrlItem>();
        foreach (var r in relations)
        {
            fromSet.Add(r.From);
            toSet.Add(r.To);
        }
        fromSet.ExceptWith(toSet);
        return fromSet;
    }

    /// <summary>
    /// Enumerates all nodes in the link tree using depth-first traversal.
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
    /// Represents a node in the link tree.
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Gets the link tree to which this node belongs.
        /// </summary>
        public LinkTree Tree { get; init; }

        /// <summary>
        /// Gets the resource (element, label, etc.) represented by this node.
        /// </summary>
        public XbrlItem Resource { get; init; }

        /// <summary>
        /// Gets the edge from the parent node to this node, if any.
        /// </summary>
        public Edge? EdgeFromParent { get; init; }

        /// <summary>
        /// Gets the order value associated with the arc from the parent edge, if available.
        /// </summary>
        /// <remarks>Returns null if there is no parent edge or if the arc does not have an order value
        /// defined.</remarks>
        public double? Order => EdgeFromParent?.Arc.Order;

        /// <summary>
        /// Gets the preferred label associated with the edge from the parent, if available.
        /// </summary>
        public string? PreferredLabel => EdgeFromParent?.Arc.PreferredLabel;

        /// <summary>
        /// Gets the arcrole associated with the current edge, if available.
        /// </summary>
        public string? Arcrole => EdgeFromParent?.Arc.Arcrole;

        /// <summary>
        /// Gets the weight of the edge from the parent node, if available.
        /// </summary>
        /// <remarks>If the node does not have a parent or the edge does not specify a weight, this
        /// property returns null.</remarks>
        public int? Weight => EdgeFromParent?.Arc.Weight;

        /// <summary>
        /// Gets the parent node, or null if this is a root node.
        /// </summary>
        public Node? Parent => EdgeFromParent?.Parent;

        /// <summary>
        /// Gets the distance from the root node (root is0).
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
        /// Gets the edges from this node to its child nodes.
        /// </summary>
        private Edge[] EdgesToChildren { get; init; }

        /// <summary>
        /// Gets an enumerable collection of child nodes, ordered by their associated arc order.
        /// </summary>
        /// <remarks>The returned collection reflects the current set of child nodes and their order as
        /// defined by the underlying edges. The enumeration is deferred and will reflect any changes to the underlying
        /// data at the time of enumeration.</remarks>
        public IEnumerable<Node> Children => EdgesToChildren.OrderBy(e => e.Arc.Order).Select(e => e.Child);

        internal Node(LinkTree tree, XbrlItem resource, Edge? fromParent)
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
    /// Represents an edge (relationship) between two nodes in the link tree.
    /// </summary>
    public class Edge
    {
        internal Edge(LinkTree tree, Node parent, XbrlItem childResource, Arc arc)
        {
            Tree = tree;
            Parent = parent;
            Arc = arc;
            Child = new Node(Tree, childResource, this);
        }

        /// <summary>
        /// Gets the link tree to which this edge belongs.
        /// </summary>
        public LinkTree Tree { get; init; }

        /// <summary>
        /// Gets the parent node of this edge.
        /// </summary>
        public Node Parent { get; init; }

        /// <summary>
        /// Gets the arc (relationship) associated with this edge.
        /// </summary>
        public Arc Arc { get; init; }

        /// <summary>
        /// Gets the child node of this edge.
        /// </summary>
        public Node Child { get; init; }
    }
}
