using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// XBRL要素
/// </summary>
public class Element : XBRLItem
{
    public enum PeriodKind
    {
        /// <summary>
        /// 期間
        /// </summary>
        Duration,

        /// <summary>
        /// 時点
        /// </summary>
        Instant,

        /// <summary>
        /// 未定義
        /// </summary>
        Undefined,
    }

    public enum BalanceKind
    {
        /// <summary>
        /// 借方
        /// </summary>
        Debit,

        /// <summary>
        /// 貸方
        /// </summary>
        Credit,

        /// <summary>
        /// 未定義
        /// </summary>
        Undefined,
    }

    public Element(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    /// <summary>
    /// 名前空間
    /// </summary>
    public XNamespace TargetNamespace => Name.Namespace;

    /// <summary>
    /// 要素名
    /// </summary>
    public required XName Name { get; init; }

    /// <summary>
    /// 抽象要素であるか
    /// </summary>
    public required bool Abstract { get; init; }

    /// <summary>
    /// 期間種別
    /// </summary>
    public required PeriodKind PeriodType { get; init; }

    /// <summary>
    /// null値許可要素であるか
    /// </summary>
    public required bool Nillable { get; init; }

    /// <summary>
    /// 代替グループ
    /// </summary>
    public required XName? SubstitutionGroup { get; init; }

    /// <summary>
    /// type
    /// </summary>
    public required XName? XBRLType { get; init; }

    /// <summary>
    /// Balance
    /// </summary>
    public required BalanceKind Balance { get; init; }
}

/// <summary>
/// タプル要素
/// </summary>
public class Tuple : Element
{
    public class Member : XBRLItem
    {
        public Member(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
        {
        }

        public required int? MaxOccurs { get; init; }
        public required int? MinOccurs { get; init; }
        public required XName ElementRef { get; init; }
        public Element Element => Dts.GetElement(ElementRef) ?? throw new InvalidDataException("Element not found.");
    }

    public Tuple(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    public required Member[] Sequence { get; init; }
}

