using Manpuku.Edinet.Xbrl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

// Group concepts by namespace
var conceptsByNamespace = dts.Concepts
    .GroupBy(concept => concept.Name.Namespace)
    .ToDictionary(group => group.Key, group => group.ToArray());

// Print concepts with labels
foreach (var ns in conceptsByNamespace.Keys)
{
    Console.WriteLine($"Namespace: {ns.NamespaceName}");
    foreach (var concept in conceptsByNamespace[ns].OrderBy(c => c.Name.LocalName))
    {
        Console.WriteLine($"\t{concept.Name.LocalName}");
        if (dts.Labels.TryGetValue(concept, out Label[]? value))
        {
            foreach (var label in value)
            {
                Console.WriteLine($"\t\t({label.Lang}) {label.Value} [{label.Role}]");
            }
        }
    }
}

// Exit program
Environment.Exit(0);

