using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Interface for an XBRL parser.
/// </summary>
public interface IXbrlParser
{
	/// <summary>
	/// Parses the specified XBRL documents and returns a discoverable taxonomy set (DTS).
	/// </summary>
	/// <param name="entryPointUri">The URI of the entry point XBRL instance file (.xbrl) or schema file (.xsd).</param>
	/// <param name="loader">A function to load an <see cref="XDocument"/> from a URI.</param>
	/// <returns>The populated <see cref="DiscoverableTaxonomySet"/>.</returns>
	Task<DiscoverableTaxonomySet> ParseAsync(Uri entryPointUri, Func<Uri, Task<XDocument>> loader);
}
