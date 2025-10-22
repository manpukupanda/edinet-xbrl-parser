namespace Manpuku.Edinet.Xbrl;

/// <summary>
/// Represents an exception that occurs during XBRL parsing.
/// </summary>
public class XbrlParseException : Exception
{
    /// <summary>
    /// Gets the message code associated with this exception.
    /// </summary>
    public string MessageCode { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="XbrlParseException"/> class with a specified error message and message code.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="messageCode">The message code.</param>
    public XbrlParseException(string message, string messageCode)
        : base(message)
    {
        MessageCode = messageCode;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="XbrlParseException"/> class with a specified error message, message code, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="messageCode">The message code.</param>
    /// <param name="inner">The exception that is the cause of the current exception.</param>
    public XbrlParseException(string message, string messageCode, Exception inner)
        : base(message, inner)
    {
        MessageCode = messageCode;
    }
}

/// <summary>
/// Represents an exception that occurs due to XBRL syntax errors during parsing.
/// </summary>
public class XbrlSyntaxException : XbrlParseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XbrlSyntaxException"/> class with a specified error message and message code.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="messageCode">The message code.</param>
    public XbrlSyntaxException(string message, string messageCode) : base(message, messageCode) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XbrlSyntaxException"/> class with a specified error message, message code, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="messageCode">The message code.</param>
    /// <param name="inner">The exception that is the cause of the current exception.</param>
    public XbrlSyntaxException(string message, string messageCode, Exception inner) : base(message, messageCode, inner) { }
}

/// <summary>
/// Represents an exception that occurs due to XBRL semantic errors during parsing.
/// </summary>
public class XbrlSemanticException : XbrlParseException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="XbrlSemanticException"/> class with a specified error message and message code.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="messageCode">The message code.</param>
    public XbrlSemanticException(string message, string messageCode) : base(message, messageCode) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="XbrlSemanticException"/> class with a specified error message, message code, and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="messageCode">The message code.</param>
    /// <param name="inner">The exception that is the cause of the current exception.</param>
    public XbrlSemanticException(string message, string messageCode, Exception inner) : base(message, messageCode, inner) { }
}

internal static class XbrlErrorCatalog
{
    public static (string Code, string Message) MissingAttribute(string attrName)
        => ("XBRL_MISSING_ATTRIBUTE", $"Missing required attribute '{attrName}'. Cannot continue parsing.");

    public static (string Code, string Message) EmptyValue(string xmltag)
        => ("XBRL_MISSING_VALUE", $"value of '{xmltag}' is empty. Cannot continue parsing.");

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

}