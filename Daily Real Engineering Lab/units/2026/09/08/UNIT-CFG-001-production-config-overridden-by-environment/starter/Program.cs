using Microsoft.Extensions.Configuration;

static void ApplyDeploymentEnvironment(string path)
{
    foreach (var raw in File.ReadAllLines(path))
    {
        var line = raw.Trim();
        if (line.Length == 0 || line.StartsWith('#')) continue;

        var separator = line.IndexOf('=');
        if (separator <= 0) continue;

        var key = line[..separator].Trim();
        var value = line[(separator + 1)..].Trim();
        Environment.SetEnvironmentVariable($"LAB_{key}", value);
    }
}

var deploymentFile = Path.Combine(AppContext.BaseDirectory, "deployment.env");
ApplyDeploymentEnvironment(deploymentFile);

var versionedProductionConfig = new Dictionary<string, string?>
{
    ["Payments:BaseUrl"] = "https://payments.prod.example/",
    ["Payments:TimeoutSeconds"] = "5"
};

var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(versionedProductionConfig)
    .AddEnvironmentVariables(prefix: "LAB_")
    .Build();

Console.WriteLine("EXPECTED_VERSIONED=https://payments.prod.example/");
Console.WriteLine($"EFFECTIVE={configuration["Payments:BaseUrl"]}");
Console.WriteLine("--- CONFIG DEBUG VIEW ---");
Console.WriteLine(configuration.GetDebugView());
