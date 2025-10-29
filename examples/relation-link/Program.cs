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

// tabspace = 2 white space
var tab = "  ";

// Print relation trees
Dictionary<RoleType, LinkTree>[] relationTreeDics =
    [dts.PresentationLinkTrees, dts.DefinitionLinkTrees, dts.DefinitionLinkTrees];

foreach (var relationTrees in relationTreeDics)
{
    var linkLind = relationTrees.First().Value.LinkKind;
    foreach (var tree in relationTrees)
    {
        var role = tree.Key;
        Console.WriteLine($"{linkLind}");
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
