var samples = new[]
{
    "https://shop.local/products/42?utm_source=newsletter",
    "https://shop.local/products/42?utm_source=ads&utm_campaign=fall",
    "https://shop.local/products/42?sort=price",
    "https://shop.local/products/42?variant=blue",
    "https://shop.local/products/42?utm_campaign=fall&variant=blue"
};

foreach (var raw in samples)
{
    Console.WriteLine($"Request:   {raw}");
    Console.WriteLine($"Canonical: {CanonicalPolicy.Build(raw)}");
    Console.WriteLine();
}

public static class CanonicalPolicy
{
    public static string Build(string rawUrl)
    {
        var uri = new Uri(rawUrl);
        // Investigation note: decide which request dimensions belong to resource identity.
        return uri.GetLeftPart(UriPartial.Path) + uri.Query;
    }
}