using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

public class XBRLItem
{
	/// <summary>
	/// 属するDTS
	/// </summary>
	public XBRLDiscoverableTaxonomySet Dts { get; init; }

	/// <summary>
	/// URI
	/// </summary>
	public Uri? URI { get; init; }

    /// <summary>
    /// XML片
    /// </summary>
    public XElement Xml { get; init; }

    /// <summary>
    /// XML片のid属性
    /// </summary>
    public string? Id { get; init; }

    public XBRLItem(XBRLDiscoverableTaxonomySet dts, XElement xml)
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
