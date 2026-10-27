using Microsoft.Extensions.Hosting;

namespace ShelterFeedLab;

public sealed class ShelterFeedWarmupService : BackgroundService
{
    private readonly WarmupProbe _probe;

    public ShelterFeedWarmupService(WarmupProbe probe) => _probe = probe;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Warmup đại diện cho bước chuẩn bị snapshot qua SDK legacy.
        _probe.LoadBlocking(stoppingToken);
        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
    }
}
