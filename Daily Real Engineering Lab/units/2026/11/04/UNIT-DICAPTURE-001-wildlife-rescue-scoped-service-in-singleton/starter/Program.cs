using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var services = new ServiceCollection();
services.AddScoped<ShiftContext>();
services.AddSingleton<NotificationWorker>();
using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = false });
var worker = provider.GetRequiredService<NotificationWorker>();
worker.Send("shift-A", "owl");
worker.Send("shift-B", "fox");

public sealed class ShiftContext
{
    private string? _shift;
    public string Shift => _shift ?? "(unset)";
    public void Initialize(string shift) => _shift ??= shift;
}
public sealed class NotificationWorker
{
    private readonly ShiftContext _context;
    public NotificationWorker(ShiftContext context) => _context = context;
    public void Send(string shift, string animal)
    {
        _context.Initialize(shift);
        Console.WriteLine($"recipient={_context.Shift};animal={animal}");
    }
}
