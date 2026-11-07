using Microsoft.Extensions.Hosting;

namespace ShelterFeedLab;

public sealed class ShelterFeedWarmupService : BackgroundService
{
    private readonly WarmupProbe _probe;

    public ShelterFeedWarmupService(WarmupProbe probe) => _probe = probe;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Trả quyền điều khiển cho host startup trước bước warmup đồng bộ.
        await Task.Yield();

        _probe.LoadBlocking(stoppingToken);
        await Task.Delay(Timeout.InfiniteTimeSpan, stoppingToken);
    }
}
