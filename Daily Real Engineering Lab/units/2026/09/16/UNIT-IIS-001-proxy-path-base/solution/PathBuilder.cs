namespace Portal;

public static class PathBuilder
{
    public static string BuildDetailsPath(string pathBase, int id)
    {
        var normalized = string.IsNullOrWhiteSpace(pathBase) || pathBase == "/"
            ? string.Empty
            : "/" + pathBase.Trim('/');
        return $"{normalized}/reports/{id}";
    }
}