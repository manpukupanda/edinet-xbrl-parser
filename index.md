# 📘 EDINET XBRL Parser for .NET

Welcome to the documentation for **EDINET XBRL Parser**, a C# class library for parsing XBRL and Inline XBRL (iXBRL) documents published by Japan's Financial Services Agency (FSA) via EDINET.

This site provides an auto-generated API reference using [docFX](https://dotnet.github.io/docfx/), based on XML documentation comments in the source code.

---
# edinet-xbrl-parser

A .NET8 library for parsing EDINET XBRL documents, including taxonomy, instance, and linkbase files. Supports both standard XBRL and Inline XBRL (iXBRL).

## Features
- Parse XBRL and Inline XBRL documents
- Discoverable Taxonomy Set (DTS) construction
- Access to elements, contexts, units, facts, labels, references, and linkbase trees
- .NET8 compatible, C#12
- XML documentation for all public APIs

## Installation

Install from NuGet:

```
dotnet add package Manpuku.Edinet.Xbrl
```

Or clone from GitHub:

```
git clone https://github.com/manpukupanda/edinet-xbrl-parser.git
```

## Usage

This library is designed for use with dependency injection (DI). You can parse EDINET XBRL data asynchronously via the IXbrlParser interface. There are two types of parsers, each with different usage.

### Standard XBRL Parser
Parse XBRL instance files (.xbrl) or schema files (.xsd) as entry points.

```csharp
services.AddSingleton<Manpuku.Edinet.Xbrl.IXbrlParser, Manpuku.Edinet.Xbrl.XbrlParser>();
```
```csharp
public interface IXbrlParser
{
 Task<XBRLDiscoverableTaxonomySet> ParseAsync(Uri entryPointUri, Func<Uri, Task<XDocument>> loader);
}
```

Example:
```csharp
var parser = serviceProvider.GetRequiredService<Manpuku.Edinet.Xbrl.IXbrlParser>();

// loader: async function to get XDocument from URI (provided by the caller)
Func<Uri, Task<XDocument>> loader = async uri =>
{
 using var httpClient = new HttpClient();
 var stream = await httpClient.GetStreamAsync(uri);
 return XDocument.Load(stream);
};

var dts = await parser.ParseAsync(new Uri("https://example.com/entrypoint.xbrl"), loader);
```

### Inline XBRL Parser
Parse Inline XBRL documents (_ixbrl.htm) as entry points.

```csharp
services.AddSingleton<Manpuku.Edinet.Xbrl.InlineXBRL.IXbrlParser, Manpuku.Edinet.Xbrl.InlineXBRL.XbrlParser>();
```
```csharp
public interface IXbrlParser
{
 Task<XBRLDiscoverableTaxonomySet> ParseInline(Uri[] inlineXBRLsURI, Func<Uri, Task<XDocument>> loader);
}
```

Example:
```csharp
var inlineParser = serviceProvider.GetRequiredService<Manpuku.Edinet.Xbrl.InlineXBRL.IXbrlParser>();

Func<Uri, Task<XDocument>> loader = async uri =>
{
 using var httpClient = new HttpClient();
 var stream = await httpClient.GetStreamAsync(uri);
 return XDocument.Load(stream);
};

var dts = await inlineParser.ParseInline(new[] {
 new Uri("https://example.com/inline1_ixbrl.htm"),
 new Uri("https://example.com/inline2_ixbrl.htm")
}, loader);
```

#### About the loader function
The `loader` function is provided by the caller and is responsible for asynchronously obtaining an `XDocument` from a given `Uri`. This design allows for flexible implementations such as HTTP retrieval, caching, or local file loading depending on your environment.

## API Reference
- All public classes and methods are documented with XML comments.
- Use IntelliSense in Visual Studio or see the source code for details.

## License

MIT License

## Links
- [GitHub Repository](https://github.com/manpukupanda/edinet-xbrl-parser)
- [API Reference](https://manpukupanda.github.io/edinet-xbrl-parser/index.html)
- [NuGet Gallery](https://www.nuget.org/packages/Manpuku.Edinet.Xbrl)
