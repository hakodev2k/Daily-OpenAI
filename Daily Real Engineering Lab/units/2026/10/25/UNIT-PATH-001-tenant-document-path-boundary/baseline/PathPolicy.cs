namespace PathBoundaryLab;

public static class DocumentPathPolicy
{
    public static string ResolveDocumentPath(string tenantRoot, string requestedPath)
    {
        if (string.IsNullOrWhiteSpace(requestedPath))
            throw new UnauthorizedAccessException("Invalid document path.");

        string root = Path.GetFullPath(tenantRoot);
        string candidate = Path.GetFullPath(Path.Combine(root, requestedPath));

        if (!candidate.StartsWith(root, StringComparison.OrdinalIgnoreCase))
            throw new UnauthorizedAccessException("Document is outside the tenant scope.");

        return candidate;
    }
}
