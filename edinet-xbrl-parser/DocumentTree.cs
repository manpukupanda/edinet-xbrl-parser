namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Represents a tree of related XBRL documents.
/// Holds the root node and provides DFS enumeration of all nodes in the tree.
/// </summary>
public class DocumentTree
{
    /// <summary>
    /// Internal constructor. Sets the root node.
    /// </summary>
    internal DocumentTree(DocumentTreeNode root)
    {
        Root = root;
    }

    /// <summary>
    /// The root node of the document tree.
    /// </summary>
    public DocumentTreeNode Root { get; private set; }

    /// <summary>
    /// Enumerates all nodes in the tree in depth-first order (root included).
    /// </summary>
    public IEnumerable<DocumentTreeNode> Nodes => TraverseDfs(Root);

    /// <summary>
    /// Helper that traverses nodes in depth-first order.
    /// </summary>
    private static IEnumerable<DocumentTreeNode> TraverseDfs(DocumentTreeNode node)
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