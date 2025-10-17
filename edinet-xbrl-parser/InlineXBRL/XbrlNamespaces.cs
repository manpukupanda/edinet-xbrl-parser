using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl.InlineXBRL;

public static class XbrlNamespaces
{
    /// <summary>
    /// http://www.xbrl.org/2008/inlineXBRL
    /// </summary>
    public static readonly XNamespace ix = "http://www.xbrl.org/2008/inlineXBRL";
    public static readonly XName ixReferences = ix + "references";
    public static readonly XName ixNonNumeric = ix + "nonNumeric";
    public static readonly XName ixNonFraction = ix + "nonFraction";

    /// <summary>
    /// http://www.w3.org/1999/xhtml
    /// </summary>
    public static readonly XNamespace xhtml = "http://www.w3.org/1999/xhtml";

    public static readonly XName format = "format";
    public static readonly XName escape = "escape";
    public static readonly XName scale = "scale";
    public static readonly XName sign = "sign";

}
