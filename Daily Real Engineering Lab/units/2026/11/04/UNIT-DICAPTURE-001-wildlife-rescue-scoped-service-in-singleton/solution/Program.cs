using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddScoped<ShiftContext>();
services.AddSingleton<NotificationWorker>();
using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });
var worker = provider.GetRequiredService<NotificationWorker>();
worker.Send("shift-A", "owl");
worker.Send("shift-B", "fox");

public sealed class ShiftContext
{
    public string Shift { get; private set; } = "(unset)";
    public void Initialize(string shift) => Shift = shift;
}
public sealed class NotificationWorker
{
    private readonly IServiceScopeFactory _scopeFactory;
    public NotificationWorker(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;
    public void Send(string shift, string animal)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ShiftContext>();
        context.Initialize(shift);
        Console.WriteLine($"recipient={context.Shift};animal={animal}");
    }
}
