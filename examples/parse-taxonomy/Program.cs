using Manpuku.Edinet.Xbrl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data;

// Before running this example, download the EDINET XBRL taxonomy files from:
// https://www.fsa.go.jp/search/20241112.html
// Look for: (c) EDINETタクソノミ本体（ZIP:5,759KB）
//
// Then extract the ZIP file to a local folder,
// and set the path to the entry point file below.
var entryPointFile = "F:\\temp\\タクソノミ\\samples\\2024-11-01\\entryPoint_jpcrp020000-srs_2024-11-01.xsd";

// Build host and register services
using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) =>
    {
        services.AddTransient<IXbrlParser, XbrlParser>(); // Register XBRL parser
    })
    .Build();

// Get XBRL parser
var parser = host.Services.GetRequiredService<IXbrlParser>();

// Create URI from local path
var entryPointUri = new Uri(Path.GetFullPath(entryPointFile));

// Parse taxonomy documents and get DTS information
var dts = await parser.ParseAsync(entryPointUri, XbrlParser.DefaultLoaderFunc);

// tabspace = 2 white space
var tab = "  ";

// Print presentation link trees
foreach (var tree in dts.PresentationLinkTrees)
{
    var role = tree.Key;
    Console.WriteLine($"{role.RoleURI}");
    Console.WriteLine($"{tab}(ja) {role.Definition}");
    Console.WriteLine($"{tab}(en) {role.DefinitionEn}");

    var roots = tree.Value.RootNodes;
    foreach (var root in roots)
    {
        PrintNodes(root, tab, true);
    }
    Console.WriteLine();
}

// Exit program
Environment.Exit(0);

// Recursive function to print relation tree nodes
void PrintNodes(LinkTree.Node node, string indent, bool isRoot = false)
{
    if (node.Resource is Concept concept)
    {
        if (isRoot)
            Console.WriteLine($"{indent}* {concept.Name.LocalName}");
        else
            Console.WriteLine($"{indent}- {node.Order} {concept.Name.LocalName} ({node.Arcrole})");
    }
    else
    {
        throw new DataException("Resource must be Concept in relation tree.");
    }

    foreach (var child in node.Children)
    {
        PrintNodes(child, indent + tab);
    }
}