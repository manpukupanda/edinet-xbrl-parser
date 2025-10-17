using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// XBRLファクト
/// </summary>
public class Fact : XBRLItem
{
    public Fact(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    /// <summary>
    /// 要素
    /// </summary>
    public required Element Element { get; init; }

    /// <summary>
    /// 値
    /// </summary>
    public required string? Value { get; init; }

    /// <summary>
    /// コンテキスト
    /// </summary>
    /// <remarks>Tupleの場合、コンテキストが存在しないのでnull。Tuple以外の場合はnullは設定されない。</remarks>
    public required Context? Context { get; init; }

    /// <summary>
    /// ユニット
    /// </summary>
    public required Unit? Unit { get; init; }

    /// <summary>
    /// nil値
    /// </summary>
    public required bool Nil { get; init; }

    /// <summary>
    /// decimals属性値
    /// </summary>
    public required int? Decimals { get; init; }
}

/// <summary>
/// タプル
/// </summary>
public class TupleFact : Fact
{
    public required Fact[] MemberFacts { get; init; }

    public TupleFact(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }
}
