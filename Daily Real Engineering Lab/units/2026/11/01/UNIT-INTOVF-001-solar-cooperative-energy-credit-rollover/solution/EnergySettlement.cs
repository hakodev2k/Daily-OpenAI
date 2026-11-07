public sealed record MeterDelta(string SiteId, int GeneratedWh);

public static class EnergySettlement
{
    public static long CalculateTotalWh(IEnumerable<MeterDelta> readings)
    {
        ArgumentNullException.ThrowIfNull(readings);

        long subtotalWh = 0;
        foreach (var reading in readings)
        {
            if (reading.GeneratedWh < 0)
                throw new ArgumentOutOfRangeException(nameof(readings), "Sản lượng phải không âm.");

            checked
            {
                subtotalWh += reading.GeneratedWh;
            }
        }

        return subtotalWh;
    }
}
