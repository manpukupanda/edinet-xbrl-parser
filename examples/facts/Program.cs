using Manpuku.Edinet.Xbrl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data;

// Build host and register services
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddTransient<IXbrlParser, XbrlParser>(); // Register XBRL parser
    })
    .Build();

// Get XBRL parser
var parser = host.Services.GetRequiredService<IXbrlParser>();

// Path to sample XBRL file
var xbrl = ".\\data\\jpcrp030000-asr-001_X99001-000_2025-03-31_01_2025-06-28.xbrl";

// Create URI from local path
var xbrlUri = new Uri(Path.GetFullPath(xbrl));

// Parse XBRL document and get DTS information
var dts = await parser.ParseAsync(xbrlUri, XbrlParser.DefaultLoaderFunc);

// Group facts by concept
var factsGroubedByConcept = dts.Facts
    .GroupBy(fact => fact.Concept)
    .ToDictionary(group => group.Key, group => group.ToArray());

// Print all facts in the DTS
foreach (var facts in factsGroubedByConcept)
{
    var concept = facts.Key;
    Console.WriteLine($"{concept.Name.LocalName}:");
    foreach (var fact in facts.Value)
    {
        if (fact.Context == null) throw new DataException("Fact context is null.");

        if (fact.Nil)
            Console.WriteLine($"  - Value: null");
        else
            Console.WriteLine($"  - Value: {ShortenValue(fact.Value)}");

        var decimals = fact.Decimals?.ToString() ?? "null";
        Console.WriteLine($"    Decimals: {decimals} ");

        var unit = fact.Unit?.Id ?? "null";
        Console.WriteLine($"    Unit: {unit} ");

        Console.WriteLine($"    Context:");
        Console.WriteLine($"      ID: {fact.Context.Id}");
        Console.WriteLine($"      StartDate: {EscapeNull(fact.Context.StartDate)}");
        Console.WriteLine($"      EndDate: {EscapeNull(fact.Context.EndDate)}");
        Console.WriteLine($"      Instant: {EscapeNull(fact.Context.Instant)}");

        if (fact.Context.Scenario.Length != 0)
        {
            Console.WriteLine($"      Scenario:");
            foreach (var dim in fact.Context.Scenario)
            {
                Console.WriteLine($"        - {dim.Dimension.Name.LocalName} : {dim.Member.Name.LocalName}");
            }
        }
    }
    Console.WriteLine();
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

static string EscapeNull(string value)
{
    if (string.IsNullOrWhiteSpace(value))
        return "null";
    return value;
}