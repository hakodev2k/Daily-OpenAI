namespace PathBoundaryLab;

public static class DocumentPathPolicy
{
    public static string ResolveDocumentPath(string tenantRoot, string requestedPath)
    {
        if (string.IsNullOrWhiteSpace(requestedPath) || Path.IsPathFullyQualified(requestedPath))
            throw new UnauthorizedAccessException("A relative document path is required.");

        string root = Path.GetFullPath(tenantRoot);
        string candidate = Path.GetFullPath(Path.Combine(root, requestedPath));
        string relative = Path.GetRelativePath(root, candidate);

        bool outside = relative == ".."
            || relative.StartsWith(".." + Path.DirectorySeparatorChar, StringComparison.Ordinal)
            || relative.StartsWith(".." + Path.AltDirectorySeparatorChar, StringComparison.Ordinal)
            || Path.IsPathRooted(relative);

        if (relative == "." || outside)
            throw new UnauthorizedAccessException("Document is outside the tenant scope.");

        return candidate;
    }
}
