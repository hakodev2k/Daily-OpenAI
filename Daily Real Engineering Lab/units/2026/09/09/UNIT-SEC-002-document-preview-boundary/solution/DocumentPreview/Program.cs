using System.Text;

var sandboxRoot = Path.Combine(Path.GetTempPath(), "engineering-lab-sec-002");
var allowedRoot = Path.Combine(sandboxRoot, "uploads");
var siblingRoot = Path.Combine(sandboxRoot, "uploads-archive");

Directory.CreateDirectory(allowedRoot);
Directory.CreateDirectory(siblingRoot);

File.WriteAllText(Path.Combine(allowedRoot, "invoice.txt"), "invoice: public-to-support", Encoding.UTF8);
File.WriteAllText(Path.Combine(siblingRoot, "payroll.txt"), "salary: confidential", Encoding.UTF8);

var service = new PreviewService(allowedRoot);

Console.WriteLine(service.ReadPreview(Path.Combine(allowedRoot, "invoice.txt")));

try
{
    Console.WriteLine(service.ReadPreview(Path.Combine(siblingRoot, "payroll.txt")));
    Console.WriteLine("SECURITY_CHECK=FAILED");
    Environment.ExitCode = 2;
}
catch (UnauthorizedAccessException)
{
    Console.WriteLine("SECURITY_CHECK=PASSED");
}

public sealed class PreviewService
{
    private readonly string _allowedRootWithSeparator;

    public PreviewService(string allowedRoot)
    {
        var canonicalRoot = Path.TrimEndingDirectorySeparator(Path.GetFullPath(allowedRoot));
        _allowedRootWithSeparator = canonicalRoot + Path.DirectorySeparatorChar;
    }

    public string ReadPreview(string requestedPath)
    {
        var fullPath = Path.GetFullPath(requestedPath);

        if (!fullPath.StartsWith(_allowedRootWithSeparator, StringComparison.Ordinal))
        {
            throw new UnauthorizedAccessException("Requested file is outside the preview directory.");
        }

        return File.ReadAllText(fullPath);
    }
}
