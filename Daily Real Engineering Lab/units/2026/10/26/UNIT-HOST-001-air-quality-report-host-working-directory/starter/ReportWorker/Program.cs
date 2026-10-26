using System.Globalization;

var station = "NorthStation";
var aqi = 42;
Console.Error.WriteLine($"job_start station={station} cwd={Environment.CurrentDirectory} base={AppContext.BaseDirectory}");
Console.WriteLine(new DailyAirQualityReportJob(new FileTemplateProvider()).Generate(station, aqi));

internal sealed class DailyAirQualityReportJob
{
    private readonly FileTemplateProvider _provider;
    public DailyAirQualityReportJob(FileTemplateProvider provider) => _provider = provider;
    public string Generate(string station, int aqi) =>
        _provider.ReadTemplate()
            .Replace("{station}", station, StringComparison.Ordinal)
            .Replace("{aqi}", aqi.ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
}
