using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

public static class XbrlNamespaces
{
    /// <summary>
    /// http://www.w3.org/XML/1998/namespace
    /// </summary>
    public static readonly XNamespace xml = "http://www.w3.org/XML/1998/namespace";
    public static readonly XName xmlLang = xml + "lang";

    /// <summary>
    /// http://www.w3.org/2001/XMLSchema-instance
    /// </summary>
    public static readonly XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

    /// <summary>
    /// Represents the XML Schema instance (xsi) 'nil' attribute name used to indicate a nil or null value in XML
    /// documents.
    /// </summary>
    /// <remarks>The 'nil' attribute is commonly used in XML serialization to explicitly specify that an element's
    /// value is null. This field can be used when working with XML documents that conform to the XML Schema specification
    /// and require explicit nil representation.</remarks>
    public static readonly XName xsiNil = xsi + "nil";

    /// <summary>
    /// http://www.w3.org/2001/XMLSchema
    /// </summary>
    public static readonly XNamespace xsd = "http://www.w3.org/2001/XMLSchema";

    public static readonly XName xsdSchema = xsd + "schema";
    public static readonly XName xsdImport = xsd + "import";
    public static readonly XName xsdElement = xsd + "element";
    public static readonly XName xsdComplexType = xsd + "complexType";
    public static readonly XName xsdSequence = xsd + "sequence";
    public static readonly XName xsdAnnotation = xsd + "annotation";
    public static readonly XName xsdAppinfo = xsd + "appinfo";

    /// <summary>
    /// http://www.xbrl.org/2003/instance
    /// </summary>
    public static readonly XNamespace xbrli = "http://www.xbrl.org/2003/instance";

    public static readonly XName xbrliXbrl = xbrli + "xbrl";
    public static readonly XName xbrliStartDate = xbrli + "startDate";
    public static readonly XName xbrliEndDate = xbrli + "endDate";
    public static readonly XName xbrliInstant = xbrli + "instant";
    public static readonly XName xbrliMeasure = xbrli + "measure";
    public static readonly XName xbrliContext = xbrli + "context";
    public static readonly XName xbrliScenario = xbrli + "scenario";
    public static readonly XName xbrliPeriodType = xbrli + "periodType";
    public static readonly XName xbrliBalance = xbrli + "balance";
    public static readonly XName xbrliTuple = xbrli + "tuple";
    public static readonly XName xbrliUnit = xbrli + "unit";

    /// <summary>
    /// http://www.w3.org/1999/xlink
    /// </summary>
    public static readonly XNamespace xlink = "http://www.w3.org/1999/xlink";
    public static readonly XName xlinkType = xlink + "type";
    public static readonly XName xlinkFrom = xlink + "from";
    public static readonly XName xlinkTo = xlink + "to";
    public static readonly XName xlinkArcrole = xlink + "arcrole";
    public static readonly XName xlinkRole = xlink + "role";
    public static readonly XName xlinkHref = xlink + "href";
    public static readonly XName xlinkLabel = xlink + "label";

    /// <summary>
    /// http://www.xbrl.org/2003/linkbase
    /// </summary>
    public static readonly XNamespace link = "http://www.xbrl.org/2003/linkbase";
    public static readonly XName linkLinkbase = link + "linkbase";
    public static readonly XName linkLabel = link + "label";
    public static readonly XName linkReference = link + "reference";
    public static readonly XName linkLoc = link + "loc";
    public static readonly XName linkRoleType = link + "roleType";
    public static readonly XName linkDefinition = link + "definition";
    public static readonly XName linkSchemaRef = link + "schemaRef";
    public static readonly XName linkLinkbaseRef = link + "linkbaseRef";
    public static readonly XName linkUsedOn = link + "usedOn";

    /// <summary>
    /// http://xbrl.org/2006/xbrldi
    /// </summary>
    public static readonly XNamespace xbrldi = "http://xbrl.org/2006/xbrldi";
    public static readonly XName xbrldiExplicitMember = xbrldi + "explicitMember";

    /// <summary>
    /// http://xbrl.org/2008/generic
    /// </summary>
    public static readonly XNamespace gen = "http://xbrl.org/2008/generic";
    public static readonly XName genLink = gen + "link";

    /// <summary>
    /// http://xbrl.org/2008/label
    /// </summary>
    public static readonly XNamespace label = "http://xbrl.org/2008/label";
    public static readonly XName labelLabel = label + "label";

    public static readonly XName dimension = "dimension";
    public static readonly XName use = "use";
    public static readonly XName priority = "priority";
    public static readonly XName order = "order";
    public static readonly XName weight = "weight";
    public static readonly XName preferredLabel = "preferredLabel";
    public static readonly XName name = "name";
    public static readonly XName @abstract = "abstract";
    public static readonly XName nillable = "nillable";
    public static readonly XName substitutionGroup = "substitutionGroup";
    public static readonly XName type = "type";
    public static readonly XName targetNamespace = "targetNamespace";
    public static readonly XName maxOccurs = "maxOccurs";
    public static readonly XName minOccurs = "minOccurs";
    public static readonly XName @ref = "ref";
    public static readonly XName contextRef = "contextRef";
    public static readonly XName unitRef = "unitRef";
    public static readonly XName decimals = "decimals";
    public static readonly XName roleURI = "roleURI";
    public static readonly XName schemaLocation = "schemaLocation";

    public static readonly string DefaultLinkRole = "http://www.xbrl.org/2003/role/link";
    public static readonly string DefaultLinkRole2008 = "http://www.xbrl.org/2008/role/link";
}
