namespace Portal;

public static class PathBuilder
{
    public static string BuildDetailsPath(string pathBase, int id)
    {
        // Investigation note: compare this output under root hosting and sub-path hosting.
        return $"/reports/{id}";
    }
}

public static class Program
{
    public static int Main(string[] args)
    {
        var pathBase = args.Length > 0 ? args[0] : "";
        Console.WriteLine(PathBuilder.BuildDetailsPath(pathBase, 42));
        return 0;
    }
}