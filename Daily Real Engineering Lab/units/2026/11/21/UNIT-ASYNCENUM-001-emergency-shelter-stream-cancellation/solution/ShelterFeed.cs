using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ShelterLab;

public static class ShelterFeed
{
    public static async IAsyncEnumerable<int> GetCapacityUpdatesAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        for (var sequence = 1; sequence <= 3; sequence++)
        {
            await Task.Delay(15, cancellationToken);
            yield return sequence;
        }
    }
}
