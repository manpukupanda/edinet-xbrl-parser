using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

public static class Extentions
{
	public static string? AttributeString(this XElement xml, XName name)
	{
		return xml.Attribute(name)?.Value;
	}

	public static int? AttributeInt(this XElement xml, XName name)
	{
		var a = xml.Attribute(name);
		if (a == null)
			return null;

		if (int.TryParse(a.Value, out var v))
			return v;

		return null;
	}

	public static double? AttributeDouble(this XElement xml, XName name)
	{
		var a = xml.Attribute(name);
		if (a == null)
			return null;

		if (double.TryParse(a.Value, out var v))
			return v;

		return null;
	}

	public static bool? AttributeBool(this XElement xml, XName name)
	{
		var a = xml.Attribute(name);
		if (a == null)
			return null;

		if (bool.TryParse(a.Value, out var v))
			return v;

		return null;
	}

	public static XName? ResolveQName(this XElement xml, string? qName)
	{
		if (string.IsNullOrWhiteSpace(qName))
			return null;

		var parts = qName.Split(':');
		if (parts.Length == 2)
		{
			var prefix = parts[0];
			var localName = parts[1];
			var ns = xml.GetNamespaceOfPrefix(prefix);

			if (ns == null)
				return null; // 名前空間が見つからない場合は null を返す

			return XName.Get(localName, ns.NamespaceName);
		}

		// プレフィックスなしの場合は、現在の要素の名前空間を使う
		return XName.Get(qName, xml.Name.NamespaceName);
	}
}
