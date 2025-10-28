using System.Collections.Immutable;
using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Represents a link element in an XBRL linkbase (presentation, definition, calculation, label, reference, footnote, or generic links).
/// </summary>
public class XBRLLink : XbrlItem
{
    /// <summary>
    /// Specifies the type of XBRL link.
    /// </summary>
    public enum LinkKind
    {
        /// <summary>
        /// Presentation link.
        /// </summary>
        presentationLink,

        /// <summary>
        /// Definition link.
        /// </summary>
        definitionLink,

        /// <summary>
        /// Calculation link.
        /// </summary>
        calculationLink,

        /// <summary>
        /// Label link.
        /// </summary>
        labelLink,

        /// <summary>
        /// Reference link.
        /// </summary>
        referenceLink,

        /// <summary>
        /// Footnote link.
        /// </summary>
        footnoteLink,

        /// <summary>
        /// Generic link.
        /// </summary>
        genericLink,
    }

    /// <summary>
    /// Gets the tag name (XName) for the specified link kind.
    /// </summary>
    /// <param name="kind">The link kind.</param>
    /// <returns>The corresponding XName for the link tag.</returns>
    public static XName TagNameOf(LinkKind kind)
    {
        if (kind == LinkKind.genericLink)
        {
            return XbrlNamespaces.genLink;
        }
        return XbrlNamespaces.link + kind.ToString();
    }

    /// <summary>
    /// Gets the kind of link.
    /// </summary>
    public required LinkKind Kind { get; init; }

    /// <summary>
    /// Gets the role type associated with this link.
    /// </summary>
    public required RoleType RoleType { get; init; }

    /// <summary>
    /// Gets the arcs contained in this link.
    /// </summary>
    public required Arc[] Arcs { get; init; }

    /// <summary>
    /// Gets the locators contained in this link.
    /// </summary>
    public required Locator[] Locators { get; init; }

    /// <summary>
    /// Gets the labels contained in this link (label resources).
    /// </summary>
    public required Label[] Labels { get; init; }

    /// <summary>
    /// Gets the reference resources contained in this link.
    /// </summary>
    public required Reference[] References { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="XBRLLink"/> class.
    /// </summary>
    /// <param name="dts">The discoverable taxonomy set this link belongs to.</param>
    /// <param name="xml">The XML element representing this link.</param>
    public XBRLLink(DiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    ImmutableDictionary<string, ImmutableList<Locator>>? _cachedLocators;

    ImmutableDictionary<string, ImmutableList<Locator>> ToCachedLocators(IEnumerable<Locator> locators)
    {
        var temp = new Dictionary<string, List<Locator>>();
        foreach (var loc in locators)
        {
            var key = loc.Label;
            temp.TryAdd(key, []);
            temp[key].Add(loc);
        }
        return temp.ToImmutableDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToImmutableList()
        );
    }

    /// <summary>
    /// Retrieves all locators associated with the specified label.
    /// </summary>
    /// <param name="label">The label used to identify and retrieve the associated locators. Cannot be null.</param>
    /// <returns>An enumerable collection of locators that are associated with the specified label. Returns an empty collection
    /// if no locators are found for the label.</returns>
    public IEnumerable<Locator> GetLocatorsByLabel(string label)
    {
        if (_cachedLocators == null)
        {
            _cachedLocators = ToCachedLocators(Locators);
        }

        if (_cachedLocators.TryGetValue(label, out ImmutableList<Locator>? value))
            foreach (var loc in value)
                yield return loc;
    }

    ImmutableDictionary<string, ImmutableList<XbrlItem>>? _cachedLabels;

    ImmutableDictionary<string, ImmutableList<XbrlItem>> ToCachedResources(IEnumerable<XbrlItem> resources)
    {
        var temp = new Dictionary<string, List<XbrlItem>>();

        foreach (var resource in resources)
        {
            var key = resource.Xml.AttributeString(XbrlNamespaces.xlinkLabel);
            if (key != null)
            {
                temp.TryAdd(key, []);
                temp[key].Add(resource);
            }
        }
        return temp.ToImmutableDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToImmutableList()
        );
    }

    ImmutableDictionary<string, ImmutableList<XbrlItem>>? _cachedReferences;


    internal IEnumerable<LinkTree.Relation> GetRelations()
    {
        foreach (var arc in Arcs)
        {
            foreach (var f in ResolveResourcesByLabel(arc.From))
            {
                foreach (var t in ResolveResourcesByLabel(arc.To))
                {
                    yield return new LinkTree.Relation(arc, f, t);
                }
            }
        }
    }

    IEnumerable<XbrlItem> ResolveResourcesByLabel(string label)
    {
        foreach (var lab in GetResourcesByLabel(label))
        {
            yield return lab;
        }
        foreach (var loc in GetLocatorsByLabel(label))
        {
            var res = loc.Resource;
            if (res != null)
            {
                yield return res;
            }
        }
    }

    /// <summary>
    /// Gets resources included in the link with the specified label, such as Label, Reference, Footnote, and others.
    /// Currently, it supports only Label and Reference.
    /// </summary>
    IEnumerable<XbrlItem> GetResourcesByLabel(string label)
    {
        if (Kind == LinkKind.labelLink || Kind == LinkKind.genericLink)
        {
            if (_cachedLabels == null)
            {
                // Create cache on first access
                _cachedLabels = ToCachedResources(Labels);
            }
            if (_cachedLabels.TryGetValue(label, out ImmutableList<XbrlItem>? value))
                foreach (var l in value)
                    yield return l;
        }
        else if (Kind == LinkKind.referenceLink)
        {
            if (_cachedReferences == null)
            {
                // Create cache on first access
                _cachedReferences = ToCachedResources(References);
            }
            if (_cachedReferences.TryGetValue(label, out ImmutableList<XbrlItem>? value))
                foreach (var r in value)
                    yield return r;
        }
    }
}
