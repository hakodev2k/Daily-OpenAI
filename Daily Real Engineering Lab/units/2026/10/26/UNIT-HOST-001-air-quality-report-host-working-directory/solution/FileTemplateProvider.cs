internal sealed class FileTemplateProvider
{
    public string ReadTemplate()
    {
        // Tài nguyên đi kèm binary phải được resolve độc lập với process working directory.
        var templatePath = Path.Combine(AppContext.BaseDirectory, "templates", "summary.txt");
        return File.ReadAllText(templatePath);
    }
}
