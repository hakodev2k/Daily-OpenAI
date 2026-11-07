internal sealed class FileTemplateProvider
{
    public string ReadTemplate()
    {
        var templatePath = Path.Combine("templates", "summary.txt");
        return File.ReadAllText(templatePath);
    }
}
