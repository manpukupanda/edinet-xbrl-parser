using Manpuku.Edinet.Xbrl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Build host and register services
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddTransient<Manpuku.Edinet.Xbrl.InlineXBRL.IXbrlParser, Manpuku.Edinet.Xbrl.InlineXBRL.XbrlParser>(); // Register Inline XBRL parser
    })
    .Build();

// Get Inline XBRL parser
var parser = host.Services.GetRequiredService<Manpuku.Edinet.Xbrl.InlineXBRL.IXbrlParser>();

// Path to sample Inline XBRL files
var xbrlDir = ".\\data";

// Get all Inline XBRL files in xbrlDir (files ending with "_ixbrl.htm")
var inlineXbrlFiles = Directory.GetFiles(xbrlDir, "*_ixbrl.htm", SearchOption.AllDirectories);

// Create URIs from file paths
var xbrlUris = inlineXbrlFiles.Select(f => new Uri(Path.GetFullPath(f))).ToArray();

// Parse Inline XBRL documents and get DTS information
var dts = await parser.ParseInlineAsync(xbrlUris, XbrlParser.DefaultLoaderFunc);

// Print all facts in the DTS
foreach (var fact in dts.Facts)
{
    Console.WriteLine($"{fact.Concept.Name.LocalName}\t{ShortenValue(fact.Value)}\t{fact.Context?.Id}");
}

// Exit program
Environment.Exit(0);

// Shorten fact value for display (removes newlines and collapses whitespace)
static string ShortenValue(string? value, int maxLength = 50)
{
    if (string.IsNullOrWhiteSpace(value))
        return string.Empty;

    // Remove all newline characters and collapse whitespace
    var normalized = System.Text.RegularExpressions.Regex.Replace(value, @"[\r\n]+", " ");
    normalized = System.Text.RegularExpressions.Regex.Replace(normalized, @"\s+", " ").Trim();

    return normalized.Length <= maxLength
        ? normalized
        : normalized[..maxLength] + "...";
}