using SolarTariffLab;
using SolarTariffLab.Starter;

var result = await ScenarioRunner.RunAsync((provider, clock) => new TariffCache(provider, clock));
Console.WriteLine(result.Report());

var mode = args.FirstOrDefault() ?? "--report";
return mode switch
{
    "--expect-bug" => result.CommonContract && result.BurstProviderCalls > 1 ? 0 : 1,
    "--expect-fixed" => result.CommonContract && result.BurstProviderCalls == 1 ? 0 : 1,
    "--report" => 0,
    _ => 2
};
