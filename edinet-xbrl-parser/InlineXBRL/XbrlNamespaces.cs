using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl.InlineXBRL;

/// <summary>
/// Provides XNamespace and XName constants for Inline XBRL processing.
/// </summary>
public static class XbrlNamespaces
{
    /// <summary>
    /// The Inline XBRL namespace (http://www.xbrl.org/2008/inlineXBRL).
    /// </summary>
    public static readonly XNamespace ix = "http://www.xbrl.org/2008/inlineXBRL";

    /// <summary>
    /// The 'references' element in the Inline XBRL namespace.
    /// </summary>
    public static readonly XName ixReferences = ix + "references";

    /// <summary>
    /// The 'nonNumeric' element in the Inline XBRL namespace.
    /// </summary>
    public static readonly XName ixNonNumeric = ix + "nonNumeric";

    /// <summary>
    /// The 'nonFraction' element in the Inline XBRL namespace.
    /// </summary>
    public static readonly XName ixNonFraction = ix + "nonFraction";

    /// <summary>
    /// The XHTML namespace (http://www.w3.org/1999/xhtml).
    /// </summary>
    public static readonly XNamespace xhtml = "http://www.w3.org/1999/xhtml";

    /// <summary>
    /// The 'format' attribute name used in Inline XBRL.
    /// </summary>
    public static readonly XName format = "format";

    /// <summary>
    /// The 'escape' attribute name used in Inline XBRL.
    /// </summary>
    public static readonly XName escape = "escape";

    /// <summary>
    /// The 'scale' attribute name used in Inline XBRL.
    /// </summary>
    public static readonly XName scale = "scale";

    /// <summary>
    /// The 'sign' attribute name used in Inline XBRL.
    /// </summary>
    public static readonly XName sign = "sign";

}
