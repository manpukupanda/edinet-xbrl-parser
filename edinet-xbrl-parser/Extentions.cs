using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Provides extension methods for working with <see cref="XElement"/> in XBRL processing.
/// </summary>
internal static class Extentions
{
    /// <summary>
    /// Returns the string value of the attribute with the specified name, or null if the attribute does not exist.
    /// </summary>
    /// <param name="xml">The XML element to search.</param>
    /// <param name="name">The attribute name.</param>
    /// <returns>The attribute value as a string, or null if not found.</returns>
    public static string? AttributeString(this XElement xml, XName name)
    {
        return xml.Attribute(name)?.Value;
    }

    /// <summary>
    /// Parses the attribute value as an int and returns the value, or null if missing or not an integer.
    /// </summary>
    /// <param name="xml">The XML element to search.</param>
    /// <param name="name">The attribute name.</param>
    /// <returns>The attribute value as an int, or null if not found or not an integer.</returns>
    public static int? AttributeInt(this XElement xml, XName name)
    {
        var a = xml.Attribute(name);
        if (a == null)
            return null;

        if (int.TryParse(a.Value, out var v))
            return v;

        return null;
    }

    /// <summary>
    /// Parses the attribute value as a double and returns the value, or null if missing or not a double.
    /// </summary>
    /// <param name="xml">The XML element to search.</param>
    /// <param name="name">The attribute name.</param>
    /// <returns>The attribute value as a double, or null if not found or not a double.</returns>
    public static double? AttributeDouble(this XElement xml, XName name)
    {
        var a = xml.Attribute(name);
        if (a == null)
            return null;

        if (double.TryParse(a.Value, out var v))
            return v;

        return null;
    }

    /// <summary>
    /// Parses the attribute value as a bool and returns the value, or null if missing or not a boolean.
    /// </summary>
    /// <param name="xml">The XML element to search.</param>
    /// <param name="name">The attribute name.</param>
    /// <returns>The attribute value as a bool, or null if not found or not a boolean.</returns>
    public static bool? AttributeBool(this XElement xml, XName name)
    {
        var a = xml.Attribute(name);
        if (a == null)
            return null;

        if (bool.TryParse(a.Value, out var v))
            return v;

        return null;
    }

    /// <summary>
    /// Resolves a QName string (possibly with a prefix) to an XName using the element's namespace context.
    /// Returns null if the QName cannot be resolved.
    /// </summary>
    /// <param name="xml">The XML element providing the namespace context.</param>
    /// <param name="qName">The QName string to resolve.</param>
    /// <returns>The resolved <see cref="XName"/>, or null if it cannot be resolved.</returns>
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
                return null;

            return XName.Get(localName, ns.NamespaceName);
        }

        // If no prefix, use the current element's namespace.
        return XName.Get(qName, xml.Name.NamespaceName);
    }
}
