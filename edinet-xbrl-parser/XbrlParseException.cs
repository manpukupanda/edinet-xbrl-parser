namespace Manpuku.Edinet.Xbrl;

public class XbrlParseException : Exception
{
    public string MessageCode { get; }

    public XbrlParseException(string message, string messageCode)
        : base(message)
    {
        MessageCode = messageCode;
    }

    public XbrlParseException(string message, string messageCode, Exception inner)
        : base(message, inner)
    {
        MessageCode = messageCode;
    }
}

public class XbrlSyntaxException : XbrlParseException
{
    public XbrlSyntaxException(string message, string messageCode) : base(message, messageCode) { }
    public XbrlSyntaxException(string message, string messageCode, Exception inner) : base(message, messageCode, inner) { }
}

public class XbrlSemanticException : XbrlParseException
{
    public XbrlSemanticException(string message, string messageCode) : base(message, messageCode) { }
    public XbrlSemanticException(string message, string messageCode, Exception inner) : base(message, messageCode, inner) { }
}

internal static class XbrlErrorCatalog
{
    public static (string Code, string Message) MissingAttribute(string attrName)
        => ("XBRL_MISSING_ATTRIBUTE", $"Missing required attribute '{attrName}'. Cannot continue parsing.");

    public static (string Code, string Message) EmptyValue(string xmltag)
        => ("XBRL_MISSING_ATTRIBUTE", $"value of '{xmltag}' is empty. Cannot continue parsing.");

    public static (string Code, string Message) UnknownFormat(string format)
        => ("IXBRL_UNKNOWN_FORMAT", $"Unknown format '{format}'. Cannot continue parsing.");

    public static (string Code, string Message) UnresolvedQName(string qname)
        => ("XBRL_UNRESOLVED_QNAME", $"Unable to resolve QName '{qname}'. Namespace context missing or invalid.");

    public static (string Code, string Message) SchemaRefNotFound()
        => ("IXBRL_SCHEMAREF_NOT_FOUND", "No schemaRef element found. Cannot resolve DTS. Parsing aborted.");

    public static (string Code, string Message) ElementNotFound(string name)
        => ("XBRL_ELEMENT_NOT_FOUND", $"Unknown element '{name}'. Cannot resolve DTS. Parsing aborted.");

    public static (string Code, string Message) ContextNotFound(string contextRef)
        => ("XBRL_CONTEXT_NOT_FOUND", $"Unknown context '{contextRef}'. Cannot resolve DTS. Parsing aborted.");

    public static (string Code, string Message) RoleTypeNotFound(string roleURI)
    => ("XBRL_ROLETYPE_NOT_FOUND", $"Unknown roleType '{roleURI}'. Cannot resolve DTS. Parsing aborted.");

    public static (string Code, string Message) InvalidLinkStructure(string linkbaseType)
        => ("XBRL101", $"Invalid {linkbaseType} linkbase structure detected. Semantic integrity compromised.");
}