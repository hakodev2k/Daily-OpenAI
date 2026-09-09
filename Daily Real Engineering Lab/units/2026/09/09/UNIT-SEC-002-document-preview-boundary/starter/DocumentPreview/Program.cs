using System.Text;

var sandboxRoot = Path.Combine(Path.GetTempPath(), "engineering-lab-sec-002");
var allowedRoot = Path.Combine(sandboxRoot, "uploads");
var siblingRoot = Path.Combine(sandboxRoot, "uploads-archive");

Directory.CreateDirectory(allowedRoot);
Directory.CreateDirectory(siblingRoot);

File.WriteAllText(Path.Combine(allowedRoot, "invoice.txt"), "invoice: public-to-support", Encoding.UTF8);
File.WriteAllText(Path.Combine(siblingRoot, "payroll.txt"), "salary: confidential", Encoding.UTF8);

var service = new PreviewService(allowedRoot);

Console.WriteLine($"Allowed root: {allowedRoot}");
Console.WriteLine("Case A: expected allowed document");
Console.WriteLine(service.ReadPreview(Path.Combine(allowedRoot, "invoice.txt")));

Console.WriteLine();
Console.WriteLine("Case B: sibling directory that shares the same textual prefix");
try
{
    Console.WriteLine(service.ReadPreview(Path.Combine(siblingRoot, "payroll.txt")));
    Console.WriteLine("SECURITY_CHECK=FAILED: file outside the intended directory was returned.");
    Environment.ExitCode = 2;
}
catch (UnauthorizedAccessException)
{
    Console.WriteLine("SECURITY_CHECK=PASSED: outside file was rejected.");
}

public sealed class PreviewService
{
    private readonly string _allowedRoot;

    public PreviewService(string allowedRoot)
    {
        _allowedRoot = Path.GetFullPath(allowedRoot);
    }

    public string ReadPreview(string requestedPath)
    {
        var fullPath = Path.GetFullPath(requestedPath);

        // Investigation note:
        // What exact filesystem relationship does this condition prove?
        if (!fullPath.StartsWith(_allowedRoot, StringComparison.Ordinal))
        {
            throw new UnauthorizedAccessException("Requested file is outside the preview directory.");
        }

        return File.ReadAllText(fullPath);
    }
}
