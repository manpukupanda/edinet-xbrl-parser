using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl.InlineXBRL;

/// <summary>
/// Interface for an Inline XBRL parser.
/// </summary>
public interface IXbrlParser
{
    /// <summary>
    /// Parses the specified Inline XBRL documents and returns a discoverable taxonomy set (DTS).
    /// </summary>
    /// <param name="inlineXBRLsURI">URIs of all Inline XBRL files to parse.</param>
    /// <param name="loader">Function to load an XDocument from a URI.</param>
    /// <returns>The populated <see cref="DiscoverableTaxonomySet"/>.</returns>
    Task<DiscoverableTaxonomySet> ParseInlineAsync(Uri[] inlineXBRLsURI, Func<Uri, Task<XDocument>> loader);
}
