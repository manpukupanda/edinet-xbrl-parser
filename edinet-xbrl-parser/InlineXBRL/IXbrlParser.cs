using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl.InlineXBRL;

public interface IXbrlParser
{
    Task<XBRLDiscoverableTaxonomySet> ParseInline(Uri[] inlineXBRLsURI, Func<Uri, Task<XDocument>> loader);
}
