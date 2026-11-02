using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options => options.SingleLine = true);
builder.Services.AddSingleton<DeliveryJournal>();
builder.Services.AddSingleton<FakeGateway>();
builder.Services.AddHostedService<NoticeWorker>();

using var host = builder.Build();
var journal = host.Services.GetRequiredService<DeliveryJournal>();
var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

await host.StartAsync();
await Task.Delay(700);
var hostStoppedEarly = lifetime.ApplicationStopping.IsCancellationRequested;
await host.StopAsync();

var attempts = journal.Attempts.ToArray();
var published = journal.Published.ToArray();
var failed = journal.Failed.ToArray();
Console.WriteLine($"hostStoppedEarly={hostStoppedEarly}");
Console.WriteLine($"attempts={string.Join(",", attempts)}");
Console.WriteLine($"published={string.Join(",", published)}");
Console.WriteLine($"failed={string.Join(",", failed)}");

var correct = !hostStoppedEarly
    && attempts.SequenceEqual(new[] { "AQ-101", "AQ-102", "AQ-103" })
    && published.SequenceEqual(new[] { "AQ-101", "AQ-103" })
    && failed.SequenceEqual(new[] { "AQ-102" });
if (correct)
{
    Console.WriteLine("LAB_RESULT=PASS");
    return 0;
}

var expectedFailure = hostStoppedEarly
    && attempts.SequenceEqual(new[] { "AQ-101", "AQ-102" })
    && published.SequenceEqual(new[] { "AQ-101" })
    && failed.Length == 0;
if (expectedFailure)
{
    Console.WriteLine("LAB_RESULT=EXPECTED_FAILURE");
    return 3;
}

Console.WriteLine("LAB_RESULT=UNEXPECTED");
return 4;
