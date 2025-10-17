using System.Collections.Immutable;
using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

public class XBRLLink : XBRLItem
{
    /// <summary>
    /// リンクの種類
    /// </summary>
    public enum LinkKind
    {
        /// <summary>
        /// 表示リンク
        /// </summary>
        presentationLink,

        /// <summary>
        /// 定義リンク
        /// </summary>
        definitionLink,

        /// <summary>
        /// 計算リンク
        /// </summary>
        calculationLink,

        /// <summary>
        /// 名称リンク
        /// </summary>
        labelLink,

        /// <summary>
        /// 参照リンク
        /// </summary>
        referenceLink,

        /// <summary>
        /// 脚注リンク
        /// </summary>
        footnoteLink,

        /// <summary>
        /// ジェネリックリンク
        /// </summary>
        genericLink,
    }

    /// <summary>
    /// 指定された種類のリンクのタグ名を取得する
    /// </summary>
    /// <param name="kind"></param>
    /// <returns></returns>
    public static XName TagNameOf(LinkKind kind)
    {
        if (kind == LinkKind.genericLink)
        {
            return XbrlNamespaces.genLink;
        }
        return XbrlNamespaces.link + kind.ToString();
    }

    /// <summary>
    /// リンクの種類
    /// </summary>
    public required LinkKind Kind { get; init; }

    /// <summary>
    /// ロールタイプ
    /// </summary>
    public required RoleType RoleType { get; init; }

    /// <summary>
    /// アーク
    /// </summary>
    public required Arc[] Arcs { get; init; }

    /// <summary>
    /// ロケータ
    /// </summary>
    public required Locator[] Locators { get; init; }

    public XBRLLink(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }


    ImmutableDictionary<string, ImmutableList<Locator>>? _cachedLocators;

    ImmutableDictionary<string, ImmutableList<Locator>> ToCachedLocators(IEnumerable<Locator> locators)
    {
        var temp = new Dictionary<string, List<Locator>>();
        foreach (var loc in locators)
        {
            var key = loc.Label;
            temp.TryAdd(key, []);
            temp[key].Add(loc);
        }
        return temp.ToImmutableDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToImmutableList()
        );
    }

    public IEnumerable<Locator> GetLocatorsByLabel(string label)
    {
        if (_cachedLocators == null)
        {
            _cachedLocators = ToCachedLocators(Locators);
        }

        if (_cachedLocators.TryGetValue(label, out ImmutableList<Locator>? value))
            foreach (var loc in value)
                yield return loc;
    }

    public required Label[] Labels { get; init; }

    ImmutableDictionary<string, ImmutableList<XBRLItem>>? _cachedLabels;

    ImmutableDictionary<string, ImmutableList<XBRLItem>> ToCachedResources(IEnumerable<XBRLItem> resources)
    {
        var temp = new Dictionary<string, List<XBRLItem>>();

        foreach (var resource in resources)
        {
            var key = resource.Xml.AttributeString(XbrlNamespaces.xlinkLabel);
            if (key != null)
            {
                temp.TryAdd(key, []);
                temp[key].Add(resource);
            }
        }
        return temp.ToImmutableDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ToImmutableList()
        );
    }

    public required Reference[] References { get; init; }

    ImmutableDictionary<string, ImmutableList<XBRLItem>>? _cachedReferences;


    internal IEnumerable<LinkTree.Relation> GetRelations()
    {
        foreach (var arc in Arcs)
        {
            foreach (var f in ResolveResourcesByLabel(arc.From))
            {
                foreach (var t in ResolveResourcesByLabel(arc.To))
                {
                    yield return new LinkTree.Relation(arc, f, t);
                }
            }
        }
    }

    IEnumerable<XBRLItem> ResolveResourcesByLabel(string label)
    {
        foreach (var lab in GetResourcesByLabel(label))
        {
            yield return lab;
        }
        foreach (var loc in GetLocatorsByLabel(label))
        {
            var res = loc.Resource;
            if (res != null)
            {
                yield return res;
            }
        }
    }

    /// <summary>
    /// リンクに含まれるリソースで、Label, Reference, Footnote, その他を取得する
    /// 現在、Label, Referenceのみ対応
    /// </summary>
    IEnumerable<XBRLItem> GetResourcesByLabel(string label)
    {
        if (Kind == LinkKind.labelLink || Kind == LinkKind.genericLink)
        {
            if (_cachedLabels == null)
            {
                // 初回アクセス時にキャッシュを作成
                _cachedLabels = ToCachedResources(Labels);
            }
            if (_cachedLabels.TryGetValue(label, out ImmutableList<XBRLItem>? value))
                foreach (var l in value)
                    yield return l;
        }
        else if (Kind == LinkKind.referenceLink)
        {
            if (_cachedReferences == null)
            {
                // 初回アクセス時にキャッシュを作成
                _cachedReferences = ToCachedResources(References);
            }
            if (_cachedReferences.TryGetValue(label, out ImmutableList<XBRLItem>? value))
                foreach (var r in value)
                    yield return r;
        }
    }
}
