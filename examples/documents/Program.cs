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

// Print document tree
var root = dts.DocumentTree.Root;
PrintDocuments(root, "");

// Exit program
Environment.Exit(0);

// Recursive function to print document tree nodes
static void PrintDocuments(DocumentTreeNode node, string indent)
{
	Console.WriteLine($"{indent}- ({NodeKindToString(node.NodeKind)}) {node.URI}");
	foreach (var child in node.Children)
	{
		PrintDocuments(child, indent + "\t");
	}
}

static string NodeKindToString(DocumentTreeNode.DocumentKind kind) => kind switch
{
	DocumentTreeNode.DocumentKind.Instance => "In",
	DocumentTreeNode.DocumentKind.TaxonomySchema => "Tx",
	DocumentTreeNode.DocumentKind.Linkbase => "Lk",
	_ => "Ot"
};