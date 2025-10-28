using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Base class for items found in XBRL taxonomies and instances (elements, labels, arcs, etc.).
/// </summary>
public class XbrlItem
{
    /// <summary>
    /// Gets the discoverable taxonomy set to which this item belongs.
    /// </summary>
    public DiscoverableTaxonomySet Dts { get; init; }

    /// <summary>
    /// Gets the base URI of the document that contains this item, if available.
    /// </summary>
    public Uri? URI { get; init; }

    /// <summary>
    /// Gets the underlying XML fragment for this item.
    /// </summary>
    public XElement Xml { get; init; }

    /// <summary>
    /// Gets the value of the 'id' attribute on the XML fragment, if present.
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="XbrlItem"/> class.
    /// </summary>
    /// <param name="dts">The discoverable taxonomy set this item belongs to.</param>
    /// <param name="xml">The XML element representing this item.</param>
    public XbrlItem(DiscoverableTaxonomySet dts, XElement xml)
    {
        Dts = dts;
        Xml = xml;
        URI = xml.Document == null ? null : Dts.GetUriFor(xml.Document);
        Id = Xml.AttributeString("id");
    }

    internal Uri ReferenceUri
    {
        get
        {
            if (URI == null) throw new InvalidOperationException("URI is null.");
            return new Uri($"{URI.AbsoluteUri}#{Id}");
        }
    }

    internal static readonly XElement DummyElement = new("dummy");
}
