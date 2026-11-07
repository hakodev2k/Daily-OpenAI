var mappings = new Dictionary<string, string>
{
    ["SKU-ALPHA"] = "Product-101",
    ["SKU-BETA"] = "Product-202"
};

var incoming = new[] { "SKU-ALPHA", "sku-alpha", "Sku-Beta", "SKU-GAMMA" };

foreach (var id in incoming)
{
    var found = mappings.TryGetValue(id, out var product);
    Console.WriteLine($"{id}|found={found}|product={product ?? "-"}");
}