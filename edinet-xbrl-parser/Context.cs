using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Represents an XBRL context, which defines the reporting period, instant, and scenario information for a specific
/// XBRL fact.
/// </summary>
/// <remarks>The <see cref="Context"/> class provides details about the time period or instant to which an XBRL
/// fact applies, as well as any associated scenario information. It includes properties for start and end dates, an
/// instant date, and scenario dimensions.</remarks>
public class Context : XbrlItem
{
    /// <summary>
    /// Represents an explicit member with a specified dimension and member concept.
    /// </summary>
    /// <remarks>This class is used to define a pair of concepts, where each concept is represented by an
    /// instance of the <see cref="Concept"/> type. Both properties are required and must be initialized during object
    /// creation.</remarks>
    public class ExplicitMember
    {
        /// <summary>
        /// Gets the dimension concept associated with this instance.
        /// </summary>
        public required Concept Dimension { get; init; }

        /// <summary>
        /// Gets the member concept associated with this instance.
        /// </summary>
        public required Concept Member { get; init; }
    }

    internal Context(DiscoverableTaxonomySet dts, XElement xml) : base(dts, xml)
    {
    }

    /// <summary>
    /// Gets the start date of the event or process in string format.
    /// </summary>
    public required string StartDate { get; init; }

    /// <summary>
    /// Gets the end date of the event or process in string format.
    /// </summary>
    public required string EndDate { get; init; }

    /// <summary>
    /// Gets the timestamp representing the exact moment in time.
    /// </summary>
    public required string Instant { get; init; }

    /// <summary>
    /// Gets the collection of explicit members that define the scenario.
    /// </summary>
    public required ExplicitMember[] Scenario { get; init; }
}
