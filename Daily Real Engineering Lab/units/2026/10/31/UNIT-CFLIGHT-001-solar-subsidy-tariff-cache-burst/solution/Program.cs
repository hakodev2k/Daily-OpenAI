using SolarTariffLab;
using SolarTariffLab.Solution;

var result = await ScenarioRunner.RunAsync((provider, clock) => new TariffCache(provider, clock));
Console.WriteLine(result.Report());
return result.CommonContract && result.BurstProviderCalls == 1 ? 0 : 1;
