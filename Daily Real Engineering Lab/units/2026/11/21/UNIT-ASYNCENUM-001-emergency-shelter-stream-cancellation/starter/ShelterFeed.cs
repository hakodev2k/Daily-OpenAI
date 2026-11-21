using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ShelterLab;

public static class ShelterFeed
{
    // The dispatch client receives updates as they arrive from the gateway.
    public static async IAsyncEnumerable<int> GetCapacityUpdatesAsync(
        CancellationToken cancellationToken = default)
    {
        for (var sequence = 1; sequence <= 3; sequence++)
        {
            await Task.Delay(15, cancellationToken);
            yield return sequence;
        }
    }
}
