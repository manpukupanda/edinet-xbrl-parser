using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Represents a tuple concept that contains a sequence of member concept references.
/// </summary>
public class Tuple : Concept
{
    /// <summary>
    /// Represents a member of a tuple, referencing another concept.
    /// </summary>
    public class Member : XbrlItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Member"/> class.
        /// </summary>
        /// <param name="dts">The discoverable taxonomy set this member belongs to.</param>
        /// <param name="xml">The XML element representing this tuple member.</param>
        public Member(DiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
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
        /// Gets the reference to the concept (QName) that this member refers to.
        /// </summary>
        public required XName ConceptRef { get; init; }

        /// <summary>
        /// Gets the resolved concept definition for the referenced QName.
        /// </summary>
        public Concept Concept
        {
            get
            {
                var concept = Dts.GetConcept(ConceptRef);
                if (concept == null)
                {
                    var (code, message) = XbrlErrorCatalog.ElementNotFound(ConceptRef.ToString());
                    throw new XbrlSemanticException(code, message);
                }
                return concept;
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tuple"/> class.
    /// </summary>
    /// <param name="dts">The discoverable taxonomy set this tuple belongs to.</param>
    /// <param name="xml">The XML element representing this tuple.</param>
    public Tuple(DiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    /// <summary>
    /// Gets the sequence of member definitions composing the tuple.
    /// </summary>
    public required Member[] Sequence { get; init; }
}
