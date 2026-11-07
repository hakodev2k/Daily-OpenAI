using Microsoft.Extensions.Configuration;

var defaults = new Dictionary<string,string?> { ["Downstream:TimeoutSeconds"] = "5" };
var config = new ConfigurationBuilder().AddInMemoryCollection(defaults).AddEnvironmentVariables().Build();
var timeout = config.GetValue<int>("Downstream:TimeoutSeconds");
Console.WriteLine($"Downstream timeout: {timeout}s");
Console.WriteLine(timeout == 30 ? "OVERRIDE_APPLIED" : "DEFAULT_STILL_ACTIVE");
