using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Chạy: dotnet run -- --inspect | --reproduce | --verify
var mode = args.Length == 0 ? "--inspect" : args[0];
var metrics = new StatusMetricRecorder();

RequestDelegate endpoint = context =>
{
    context.Response.StatusCode = context.Request.Path.Value switch
    {
        "/v1/bulletins/current" => StatusCodes.Status200OK,
        "/v1/bulletins/limited" => StatusCodes.Status429TooManyRequests,
        "/v1/bulletins/partner" => StatusCodes.Status503ServiceUnavailable,
        _ => StatusCodes.Status404NotFound
    };
    return Task.CompletedTask;
};

var pipeline = new BulletinStatusMetricMiddleware(endpoint, metrics);
var cases = new (string Path, int Expected)[]
{
    ("/v1/bulletins/current", 200),
    ("/v1/bulletins/limited", 429),
    ("/v1/bulletins/partner", 503),
    ("/v1/bulletins/unknown", 404)
};

var responses = new List<(string Path, int Expected, int Actual)>();
foreach (var sample in cases)
{
    var context = new DefaultHttpContext();
    context.Request.Path = sample.Path;
    await pipeline.InvokeAsync(context);
    responses.Add((sample.Path, sample.Expected, context.Response.StatusCode));
}

Console.WriteLine("CLIENT RESPONSE / RECORDED METRIC");
for (var i = 0; i < responses.Count; i++)
{
    Console.WriteLine($"{responses[i].Path}: client={responses[i].Actual}, metric={metrics.Samples[i].StatusCode}");
}

var correctResponses = responses.All(x => x.Expected == x.Actual);
var correctMetrics = metrics.Samples.Count == responses.Count
    && responses.Select((x, i) => x.Path == metrics.Samples[i].Path
        && x.Actual == metrics.Samples[i].StatusCode).All(x => x);
var expectedSymptom = correctResponses
    && metrics.Samples.Count == 4
    && metrics.Samples[1].StatusCode == 200
    && metrics.Samples[2].StatusCode == 200
    && metrics.Samples[3].StatusCode == 200;

Console.WriteLine($"Response contract: {(correctResponses ? "PASS" : "FAIL")}");
Console.WriteLine($"Metric matches client: {(correctMetrics ? "PASS" : "FAIL")}");

return mode switch
{
    "--inspect" => correctResponses ? 0 : 1,
    "--reproduce" => expectedSymptom ? 0 : 1,
    "--verify" => correctResponses && correctMetrics ? 0 : 1,
    _ => 2
};

sealed record StatusMetricSample(string Path, int StatusCode);

sealed class StatusMetricRecorder
{
    private readonly List<StatusMetricSample> _samples = new();
    public IReadOnlyList<StatusMetricSample> Samples => _samples;

    public void Record(string path, int statusCode)
        => _samples.Add(new StatusMetricSample(path, statusCode));
}

sealed class BulletinStatusMetricMiddleware
{
    private readonly RequestDelegate _next;
    private readonly StatusMetricRecorder _recorder;

    public BulletinStatusMetricMiddleware(RequestDelegate next, StatusMetricRecorder recorder)
    {
        _next = next;
        _recorder = recorder;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);
        _recorder.Record(context.Request.Path.Value ?? "", context.Response.StatusCode);
    }
}
