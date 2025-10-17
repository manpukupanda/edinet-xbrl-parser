﻿using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl.InlineXBRL;

public class XbrlParser : Manpuku.Edinet.Xbrl.XbrlParser, IXbrlParser
{
    public XbrlParser(ILoggerFactory loggerFactory) : base(loggerFactory)
    {
    }

    public sealed override async Task<Manpuku.Edinet.Xbrl.XBRLDiscoverableTaxonomySet> ParseAsync(Uri entryPointUri, Func<Uri, Task<XDocument>> loader)
    {
        throw new InvalidOperationException("Use ParseInline instead of Parse for Inline XBRL documents.");
    }

    public async Task<XBRLDiscoverableTaxonomySet> ParseInline(Uri[] inlineXBRLsURI, Func<Uri, Task<XDocument>> loader)
    {
        _logger.LogTrace("start DTS load.");
        var dts = await LoadDtsAsync(inlineXBRLsURI, loader);
        _logger.LogTrace("end DTS load.");
        _logger.LogTrace("start parse.");
        Parse(dts);
        _logger.LogTrace("end parse.");
        return dts;
    }

    protected async Task<XBRLDiscoverableTaxonomySet> LoadDtsAsync(Uri[] inlineXBRLsURI, Func<Uri, Task<XDocument>> loader)
    {
        // 非同期で XDocument を取得
        var loadTasks = inlineXBRLsURI.Select(async uri =>
        {
            var doc = await loader(uri);
            if (doc.Root == null)
            {
                throw new InvalidDataException($"Document root is null. {uri}");
            }
            return new { uri, doc };
        });

        var ixbrlDocs = await Task.WhenAll(loadTasks);

        // schemaRef の URI を抽出
        Uri? schema = null;

        foreach (var x in ixbrlDocs)
        {
            var references = x.doc.Descendants(XbrlNamespaces.ixReferences);
            foreach (var refElement in references)
            {
                var schemaRefs = refElement.Elements(Manpuku.Edinet.Xbrl.XbrlNamespaces.linkSchemaRef);
                foreach (var schemaRef in schemaRefs)
                {
                    var href = schemaRef.AttributeString(Manpuku.Edinet.Xbrl.XbrlNamespaces.xlinkHref);
                    if (!string.IsNullOrEmpty(href))
                    {
                        schema = new Uri(x.uri, href);
                        break;
                    }
                }
                if (schema != null) break;
            }
            if (schema != null) break;
        }

        if (schema == null)
        {
            var (code, message) = XbrlErrorCatalog.SchemaRefNotFound();
            throw new XbrlParseException(message, code);
        }

        var documentTreeLoader = new DocumentTreeLoader(_loggerFactory, loader);
        var tree = await documentTreeLoader.CreateAsync(schema, null);
        var dts = new XBRLDiscoverableTaxonomySet()
        {
            DocumentTree = tree,
            InlineXBRLs = [.. ixbrlDocs.Select(x => (x.uri, x.doc))],
        };

        return dts;
    }

    private protected override (XbrlSchemaParser SchemaParser, Manpuku.Edinet.Xbrl.XbrlInstanceParser InstanceParser, XbrlLinkbaseParser LinkbaseParser) CreateParsers(Manpuku.Edinet.Xbrl.XBRLDiscoverableTaxonomySet dts)
    {
        return (new XbrlSchemaParser(dts, _loggerFactory),
                new XbrlInstanceParser((XBRLDiscoverableTaxonomySet)dts, _loggerFactory),
                new XbrlLinkbaseParser(dts, _loggerFactory));
    }
}

internal class XbrlInstanceParser : Manpuku.Edinet.Xbrl.XbrlInstanceParser
{
    public XbrlInstanceParser(XBRLDiscoverableTaxonomySet dts, ILoggerFactory loggerFactory) : base(dts, loggerFactory)
    {
    }

    protected override IEnumerable<Context> ParseContext()
    {
        var result = new List<Context>();
        var DtsInline = (XBRLDiscoverableTaxonomySet)Dts;

        foreach (var doc in DtsInline.InlineXBRLs.Select(i => i.Document))
        {
            foreach (var c in doc.Descendants(Manpuku.Edinet.Xbrl.XbrlNamespaces.xbrliContext))
            {
                var context = new Context(Dts, c)
                {
                    StartDate = ParseDate(c, Manpuku.Edinet.Xbrl.XbrlNamespaces.xbrliStartDate),
                    EndDate = ParseDate(c, Manpuku.Edinet.Xbrl.XbrlNamespaces.xbrliEndDate),
                    Instant = ParseDate(c, Manpuku.Edinet.Xbrl.XbrlNamespaces.xbrliInstant),
                    Scenario = ParseScenario(c),
                };

                result.Add(context);
            }
        }

        return result;
    }

    protected override IEnumerable<Unit> ParseUnit()
    {
        var result = new List<Unit>();
        var DtsInline = (XBRLDiscoverableTaxonomySet)Dts;

        foreach (var doc in DtsInline.InlineXBRLs.Select(i => i.Document))
        {
            var elements = doc.Descendants(Manpuku.Edinet.Xbrl.XbrlNamespaces.xbrliUnit);
            foreach (var xml in elements)
            {
                var unit = new Unit(Dts, xml);
                result.Add(unit);
            }
        }

        return result;
    }

    protected override IEnumerable<Fact> ParseFact()
    {
        var result = new List<Fact>();
        var DtsInline = (XBRLDiscoverableTaxonomySet)Dts;

        foreach (var doc in DtsInline.InlineXBRLs.Select(i => i.Document))
        {
            foreach (var e in doc.Descendants())
            {
                if (e.Name == XbrlNamespaces.ixNonNumeric || e.Name == XbrlNamespaces.ixNonFraction)
                {
                    var fact = new Fact(Dts, e)
                    {
                        Element = GetElement(e),
                        Value = ParseValue(e),
                        Nil = ParseNil(e),
                        Context = GetContext(e),
                        Unit = GetUnit(e),
                        Decimals = ParseDecimals(e),
                    };
                    result.Add(fact);
                }
            }
        }

        return result;
    }

    Element GetElement(XElement xml)
    {
        var name = xml.AttributeString("name");
        if (name == null)
        {
            var (code, message) = XbrlErrorCatalog.MissingAttribute("name");
            throw new XbrlParseException(message, code);
        }
        var xname = xml.ResolveQName(name);
        if (xname == null)
        {
            var (code, message) = XbrlErrorCatalog.UnresolvedQName(name);
            throw new XbrlParseException(message, code);
        }
        var element = Dts.GetElement(xname);
        if (element == null)
        {
            var (code, message) = XbrlErrorCatalog.UnresolvedQName(name);
            throw new XbrlParseException(message, code);
        }
        return element;
    }

    string? ParseValue(XElement xml)
    {
        if (ParseNil(xml)) { return null; }

        string? value;
        var format = xml.Attribute(XbrlNamespaces.format)?.Value;
        if (format != null)
        {
            value = TransformValue(xml, format);
        }
        else
        {
            value = xml.Attribute(XbrlNamespaces.escape)?.Value == "true"
                ? string.Concat(ExtractEspacedContent(xml).Select(n => n.ToString()))
                : xml.Value;
        }

        var scale = xml.Attribute(XbrlNamespaces.scale);
        if (value != null && scale != null && scale.Value != "0")
        {
            if (int.TryParse(scale.Value, out var scaleVal) &&
                decimal.TryParse(value, out decimal v))
            {
                value = (v * (decimal)Math.Pow(10, scaleVal)).ToString();
            }
        }

        var sign = xml.Attribute(XbrlNamespaces.sign);
        if (sign != null && sign.Value == "-")
        {
            value = "-" + value;
        }
        return value;
    }

    static string? TransformValue(XElement xml, string format)
    {
        var formatName = xml.ResolveQName(format);
        if (formatName != null)
        {
            if (formatName.LocalName == "dateerayearmonthdayjp" || formatName.LocalName == "dateyearmonthdaycjk")
            {
                if (string.IsNullOrEmpty(xml.Value))
                {
                    return null;
                }

                if (TryParseJapaneseDate(xml.Value, out var d))
                {
                    return d.ToString("yyyy-MM-dd");
                }
                return xml.Value;
            }
            else if (formatName.LocalName == "numdotdecimal")
            {
                if (string.IsNullOrEmpty(xml.Value))
                {
                    return null;
                }

                var num = ToHankakuNum(xml.Value);
                if (decimal.TryParse(num, out var v))
                {
                    return v.ToString();
                }
                else
                {
                    return xml.Value;
                }
            }
            else if (formatName.LocalName == "booleantrue")
            {
                return "true";
            }
            else if (formatName.LocalName == "booleanfalse")
            {
                return "false";
            }
            else if (formatName.LocalName == "numunitdecimal")
            {
                var s = xml.Value;
                if (string.IsNullOrEmpty(s))
                {
                    return null;
                }

                s = ToHankakuNum(s).Replace(" ", "");

                var match = System.Text.RegularExpressions.Regex.Match(s, @"^([0-9,]*)[^0-9]+([0-9]*)");
                if (match.Success)
                {
                    var v = decimal.Parse(match.Groups[1].Value + "." + match.Groups[2].Value);
                    return v.ToString();
                }
                else
                {
                    return xml.Value;
                }
            }
        }
        var (code, message) = XbrlErrorCatalog.UnknownFormat(format);
        throw new XbrlParseException(message, code);
    }

    static bool TryParseJapaneseDate(string input, out DateTime result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(input))
            return false;

        var normalized = input
            .Replace("元年", "1年")
            .Normalize(NormalizationForm.FormKC);

        normalized = ToHankakuNum(normalized);

        // 西暦としてまず試す
        if (DateTime.TryParse(normalized, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
        {
            return true;
        }

        // 和暦として試す
        var jaCulture = new CultureInfo("ja-JP");
        jaCulture.DateTimeFormat.Calendar = new JapaneseCalendar();

        return DateTime.TryParse(normalized, jaCulture, DateTimeStyles.AssumeLocal, out result);
    }

    private static readonly Dictionary<char, char> ZenkakuToHankakuMap = new()
    {
        ['０'] = '0',
        ['１'] = '1',
        ['２'] = '2',
        ['３'] = '3',
        ['４'] = '4',
        ['５'] = '5',
        ['６'] = '6',
        ['７'] = '7',
        ['８'] = '8',
        ['９'] = '9'
    };

    static string ToHankakuNum(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var result = new StringBuilder(input.Length);
        foreach (var c in input)
        {
            result.Append(ZenkakuToHankakuMap.TryGetValue(c, out var half) ? half : c);
        }

        return result.ToString();
    }

    static XNode[] ExtractEspacedContent(XElement element)
    {
        return element.Name.Namespace == XbrlNamespaces.ix
            ? UnwrapIxElement(element)
            : CloneElementWithoutNamespace(element);
    }

    static XNode[] UnwrapIxElement(XElement element)
    {
        var nodes = new List<XNode>();
        foreach (var node in element.Nodes())
        {
            if (node is XElement child)
            {
                nodes.AddRange(ExtractEspacedContent(child));
            }
            else
            {
                nodes.Add(node);
            }
        }
        return [.. nodes];
    }

    static XNode[] CloneElementWithoutNamespace(XElement element)
    {
        var clone = new XElement(element.Name.LocalName);

        foreach (var attr in element.Attributes())
        {
            if (!attr.IsNamespaceDeclaration)
            {
                clone.Add(attr);
            }
        }

        foreach (var node in element.Nodes())
        {
            if (node is XElement child)
            {
                foreach (var n in ExtractEspacedContent(child))
                {
                    clone.Add(n);
                }
            }
            else
            {
                clone.Add(node);
            }
        }

        return [clone];
    }
}