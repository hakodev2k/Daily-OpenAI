var samples = new[] { "https://shop.local/products/42?utm_source=newsletter", "https://shop.local/products/42?utm_source=ads&utm_campaign=fall", "https://shop.local/products/42?sort=price", "https://shop.local/products/42?variant=blue", "https://shop.local/products/42?utm_campaign=fall&variant=blue" };
foreach (var raw in samples) { Console.WriteLine($"Request:   {raw}"); Console.WriteLine($"Canonical: {CanonicalPolicy.Build(raw)}"); Console.WriteLine(); }
public static class CanonicalPolicy
{
    public static string Build(string rawUrl)
    {
        var uri = new Uri(rawUrl);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        var variant = query["variant"];
        var canonical = uri.GetLeftPart(UriPartial.Path);
        return string.IsNullOrWhiteSpace(variant) ? canonical : $"{canonical}?variant={Uri.EscapeDataString(variant)}";
    }
}