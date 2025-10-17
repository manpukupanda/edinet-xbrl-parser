using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

public class Arc : XBRLItem
{
    public enum UseKind
    {
        /// <summary>
        /// このアークが，DTS において，アークによって示される関係のネットワークに参加
        /// してもよい一つの関係又は関係の集合を表すことを示す
        /// </summary>
        optional,

        /// <summary>
        /// このアークが，DTS において，アークによって示される関係のネットワークに，
        /// それら自身及び他の対等関係を含めることを禁止する一つの関係又は関係の集合を表すことを示す。
        /// </summary>
        prohibited,
    }

    internal Arc(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    /// <summary>
    /// from属性値
    /// </summary>
    public required string From { get; init; }

    /// <summary>
    /// to属性値
    /// </summary>
    public required string To { get; init; }

    /// <summary>
    /// use属性
    /// </summary>
    public required UseKind Use { get; init; }

    /// <summary>
    /// priority属性
    /// </summary>
    public required int Priority { get; init; }

    /// <summary>
    /// arcrole属性
    /// </summary>
    public required string? Arcrole { get; init; }

    /// <summary>
    /// order属性
    /// </summary>
    public required double Order { get; init; }

    /// <summary>
    /// weight属性
    /// </summary>
    public required int? Weight { get; init; }

    /// <summary>
    /// preferredLabel属性
    /// </summary>
    public required string? PreferredLabel { get; init; }
}
