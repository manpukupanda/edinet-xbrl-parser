using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Represents a tuple element that contains a sequence of member element references.
/// </summary>
public class Tuple : Element
{
    /// <summary>
    /// Represents a member of a tuple, referencing another element.
    /// </summary>
    public class Member : XbrlItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Member"/> class.
        /// </summary>
        /// <param name="dts">The discoverable taxonomy set this member belongs to.</param>
        /// <param name="xml">The XML element representing this tuple member.</param>
        public Member(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
        {
        }

        /// <summary>
        /// Gets the maximum occurrences allowed for the member.
        /// </summary>
        public required int? MaxOccurs { get; init; }

        /// <summary>
        /// Gets the minimum occurrences allowed for the member.
        /// </summary>
        public required int? MinOccurs { get; init; }

        /// <summary>
        /// Gets the reference to the element (QName) that this member refers to.
        /// </summary>
        public required XName ElementRef { get; init; }

        /// <summary>
        /// Gets the resolved element definition for the referenced QName.
        /// </summary>
        public Element Element
        {
            get
            {
                var elemnt = Dts.GetElement(ElementRef);
                if (elemnt == null)
                {
                    var (code, message) = XbrlErrorCatalog.ElementNotFound(ElementRef.ToString());
                    throw new XbrlSemanticException(code, message);
                }
                return elemnt;
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tuple"/> class.
    /// </summary>
    /// <param name="dts">The discoverable taxonomy set this tuple belongs to.</param>
    /// <param name="xml">The XML element representing this tuple.</param>
    public Tuple(XBRLDiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    /// <summary>
    /// Gets the sequence of member definitions composing the tuple.
    /// </summary>
    public required Member[] Sequence { get; init; }
}
