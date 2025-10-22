using System.Xml.Linq;

namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Provides XNamespace and XName constants for XBRL, XML Schema, XLink, and related namespaces used in XBRL processing.
/// </summary>
public static class XbrlNamespaces
{
    /// <summary>
    /// The XML namespace (http://www.w3.org/XML/1998/namespace).
    /// </summary>
    public static readonly XNamespace xml = "http://www.w3.org/XML/1998/namespace";
    /// <summary>
    /// The xml:lang attribute name.
    /// </summary>
    public static readonly XName xmlLang = xml + "lang";

    /// <summary>
    /// The XML Schema Instance namespace (http://www.w3.org/2001/XMLSchema-instance).
    /// </summary>
    public static readonly XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

    /// <summary>
    /// The xsi:nil attribute name used to indicate a nil or null value in XML documents.
    /// </summary>
    /// <remarks>The 'nil' attribute is commonly used in XML serialization to explicitly specify that an element's value is null. This field can be used when working with XML documents that conform to the XML Schema specification and require explicit nil representation.</remarks>
    public static readonly XName xsiNil = xsi + "nil";

    /// <summary>
    /// The XML Schema namespace (http://www.w3.org/2001/XMLSchema).
    /// </summary>
    public static readonly XNamespace xsd = "http://www.w3.org/2001/XMLSchema";
    /// <summary>
    /// The xsd:schema element name.
    /// </summary>
    public static readonly XName xsdSchema = xsd + "schema";
    /// <summary>
    /// The xsd:import element name.
    /// </summary>
    public static readonly XName xsdImport = xsd + "import";
    /// <summary>
    /// The xsd:element element name.
    /// </summary>
    public static readonly XName xsdElement = xsd + "element";
    /// <summary>
    /// The xsd:complexType element name.
    /// </summary>
    public static readonly XName xsdComplexType = xsd + "complexType";
    /// <summary>
    /// The xsd:sequence element name.
    /// </summary>
    public static readonly XName xsdSequence = xsd + "sequence";
    /// <summary>
    /// The xsd:annotation element name.
    /// </summary>
    public static readonly XName xsdAnnotation = xsd + "annotation";
    /// <summary>
    /// The xsd:appinfo element name.
    /// </summary>
    public static readonly XName xsdAppinfo = xsd + "appinfo";

    /// <summary>
    /// The XBRL instance namespace (http://www.xbrl.org/2003/instance).
    /// </summary>
    public static readonly XNamespace xbrli = "http://www.xbrl.org/2003/instance";
    /// <summary>
    /// The xbrli:xbrl element name.
    /// </summary>
    public static readonly XName xbrliXbrl = xbrli + "xbrl";
    /// <summary>
    /// The xbrli:startDate element name.
    /// </summary>
    public static readonly XName xbrliStartDate = xbrli + "startDate";
    /// <summary>
    /// The xbrli:endDate element name.
    /// </summary>
    public static readonly XName xbrliEndDate = xbrli + "endDate";
    /// <summary>
    /// The xbrli:instant element name.
    /// </summary>
    public static readonly XName xbrliInstant = xbrli + "instant";
    /// <summary>
    /// The xbrli:measure element name.
    /// </summary>
    public static readonly XName xbrliMeasure = xbrli + "measure";
    /// <summary>
    /// The xbrli:context element name.
    /// </summary>
    public static readonly XName xbrliContext = xbrli + "context";
    /// <summary>
    /// The xbrli:scenario element name.
    /// </summary>
    public static readonly XName xbrliScenario = xbrli + "scenario";
    /// <summary>
    /// The xbrli:periodType element name.
    /// </summary>
    public static readonly XName xbrliPeriodType = xbrli + "periodType";
    /// <summary>
    /// The xbrli:balance element name.
    /// </summary>
    public static readonly XName xbrliBalance = xbrli + "balance";
    /// <summary>
    /// The xbrli:tuple element name.
    /// </summary>
    public static readonly XName xbrliTuple = xbrli + "tuple";
    /// <summary>
    /// The xbrli:unit element name.
    /// </summary>
    public static readonly XName xbrliUnit = xbrli + "unit";

    /// <summary>
    /// The XLink namespace (http://www.w3.org/1999/xlink).
    /// </summary>
    public static readonly XNamespace xlink = "http://www.w3.org/1999/xlink";
    /// <summary>
    /// The xlink:type attribute name.
    /// </summary>
    public static readonly XName xlinkType = xlink + "type";
    /// <summary>
    /// The xlink:from attribute name.
    /// </summary>
    public static readonly XName xlinkFrom = xlink + "from";
    /// <summary>
    /// The xlink:to attribute name.
    /// </summary>
    public static readonly XName xlinkTo = xlink + "to";
    /// <summary>
    /// The xlink:arcrole attribute name.
    /// </summary>
    public static readonly XName xlinkArcrole = xlink + "arcrole";
    /// <summary>
    /// The xlink:role attribute name.
    /// </summary>
    public static readonly XName xlinkRole = xlink + "role";
    /// <summary>
    /// The xlink:href attribute name.
    /// </summary>
    public static readonly XName xlinkHref = xlink + "href";
    /// <summary>
    /// The xlink:label attribute name.
    /// </summary>
    public static readonly XName xlinkLabel = xlink + "label";

    /// <summary>
    /// The XBRL linkbase namespace (http://www.xbrl.org/2003/linkbase).
    /// </summary>
    public static readonly XNamespace link = "http://www.xbrl.org/2003/linkbase";
    /// <summary>
    /// The link:linkbase element name.
    /// </summary>
    public static readonly XName linkLinkbase = link + "linkbase";
    /// <summary>
    /// The link:label element name.
    /// </summary>
    public static readonly XName linkLabel = link + "label";
    /// <summary>
    /// The link:reference element name.
    /// </summary>
    public static readonly XName linkReference = link + "reference";
    /// <summary>
    /// The link:loc element name.
    /// </summary>
    public static readonly XName linkLoc = link + "loc";
    /// <summary>
    /// The link:roleType element name.
    /// </summary>
    public static readonly XName linkRoleType = link + "roleType";
    /// <summary>
    /// The link:definition element name.
    /// </summary>
    public static readonly XName linkDefinition = link + "definition";
    /// <summary>
    /// The link:schemaRef element name.
    /// </summary>
    public static readonly XName linkSchemaRef = link + "schemaRef";
    /// <summary>
    /// The link:linkbaseRef element name.
    /// </summary>
    public static readonly XName linkLinkbaseRef = link + "linkbaseRef";
    /// <summary>
    /// The link:usedOn element name.
    /// </summary>
    public static readonly XName linkUsedOn = link + "usedOn";

    /// <summary>
    /// The XBRL dimensions namespace (http://xbrl.org/2006/xbrldi).
    /// </summary>
    public static readonly XNamespace xbrldi = "http://xbrl.org/2006/xbrldi";
    /// <summary>
    /// The xbrldi:explicitMember element name.
    /// </summary>
    public static readonly XName xbrldiExplicitMember = xbrldi + "explicitMember";

    /// <summary>
    /// The XBRL generic link namespace (http://xbrl.org/2008/generic).
    /// </summary>
    public static readonly XNamespace gen = "http://xbrl.org/2008/generic";
    /// <summary>
    /// The gen:link element name.
    /// </summary>
    public static readonly XName genLink = gen + "link";

    /// <summary>
    /// The XBRL label link namespace (http://xbrl.org/2008/label).
    /// </summary>
    public static readonly XNamespace label = "http://xbrl.org/2008/label";
    /// <summary>
    /// The label:label element name.
    /// </summary>
    public static readonly XName labelLabel = label + "label";

    /// <summary>
    /// The 'dimension' attribute or element name.
    /// </summary>
    public static readonly XName dimension = "dimension";
    /// <summary>
    /// The 'use' attribute or element name.
    /// </summary>
    public static readonly XName use = "use";
    /// <summary>
    /// The 'priority' attribute or element name.
    /// </summary>
    public static readonly XName priority = "priority";
    /// <summary>
    /// The 'order' attribute or element name.
    /// </summary>
    public static readonly XName order = "order";
    /// <summary>
    /// The 'weight' attribute or element name.
    /// </summary>
    public static readonly XName weight = "weight";
    /// <summary>
    /// The 'preferredLabel' attribute or element name.
    /// </summary>
    public static readonly XName preferredLabel = "preferredLabel";
    /// <summary>
    /// The 'name' attribute or element name.
    /// </summary>
    public static readonly XName name = "name";
    /// <summary>
    /// The 'abstract' attribute or element name.
    /// </summary>
    public static readonly XName @abstract = "abstract";
    /// <summary>
    /// The 'nillable' attribute or element name.
    /// </summary>
    public static readonly XName nillable = "nillable";
    /// <summary>
    /// The 'substitutionGroup' attribute or element name.
    /// </summary>
    public static readonly XName substitutionGroup = "substitutionGroup";
    /// <summary>
    /// The 'type' attribute or element name.
    /// </summary>
    public static readonly XName type = "type";
    /// <summary>
    /// The 'targetNamespace' attribute or element name.
    /// </summary>
    public static readonly XName targetNamespace = "targetNamespace";
    /// <summary>
    /// The 'maxOccurs' attribute or element name.
    /// </summary>
    public static readonly XName maxOccurs = "maxOccurs";
    /// <summary>
    /// The 'minOccurs' attribute or element name.
    /// </summary>
    public static readonly XName minOccurs = "minOccurs";
    /// <summary>
    /// The 'ref' attribute or element name.
    /// </summary>
    public static readonly XName @ref = "ref";
    /// <summary>
    /// The 'contextRef' attribute or element name.
    /// </summary>
    public static readonly XName contextRef = "contextRef";
    /// <summary>
    /// The 'unitRef' attribute or element name.
    /// </summary>
    public static readonly XName unitRef = "unitRef";
    /// <summary>
    /// The 'decimals' attribute or element name.
    /// </summary>
    public static readonly XName decimals = "decimals";
    /// <summary>
    /// The 'roleURI' attribute or element name.
    /// </summary>
    public static readonly XName roleURI = "roleURI";
    /// <summary>
    /// The 'schemaLocation' attribute or element name.
    /// </summary>
    public static readonly XName schemaLocation = "schemaLocation";

    /// <summary>
    /// The default link role URI (http://www.xbrl.org/2003/role/link).
    /// </summary>
    public static readonly string DefaultLinkRole = "http://www.xbrl.org/2003/role/link";
    /// <summary>
    /// The default link role URI for2008 (http://www.xbrl.org/2008/role/link).
    /// </summary>
    public static readonly string DefaultLinkRole2008 = "http://www.xbrl.org/2008/role/link";
}
