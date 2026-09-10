var databaseTimeline = new[] { true, true, false, false, true, true };
var restartCount = 0;

foreach (var databaseHealthy in databaseTimeline)
{
    var readinessHealthy = databaseHealthy;
    var livenessHealthy = databaseHealthy;

    if (!livenessHealthy)
    {
        restartCount++;
    }

    Console.WriteLine($"db={(databaseHealthy ? "UP" : "DOWN")} ready={readinessHealthy} live={livenessHealthy} restarts={restartCount}");
}

Console.WriteLine($"RESULT restartCount={restartCount}");
