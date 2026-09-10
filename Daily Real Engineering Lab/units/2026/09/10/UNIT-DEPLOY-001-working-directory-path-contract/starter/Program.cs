var templatePath = Path.Combine("templates", "invoice.txt");
Console.WriteLine($"CURRENT_DIR={Environment.CurrentDirectory}");
Console.WriteLine($"TEMPLATE_PATH={Path.GetFullPath(templatePath)}");

if (!File.Exists(templatePath))
{
    Console.WriteLine("Template not found");
    Environment.ExitCode = 2;
    return;
}

var template = await File.ReadAllTextAsync(templatePath);
Console.WriteLine(template.Replace("{{customer}}", "ACME").Trim());
