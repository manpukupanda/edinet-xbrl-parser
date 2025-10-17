using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

public interface IXbrlParser
{
    Task<XBRLDiscoverableTaxonomySet> ParseAsync(Uri entryPointUri, Func<Uri, Task<XDocument>> loader);
}
