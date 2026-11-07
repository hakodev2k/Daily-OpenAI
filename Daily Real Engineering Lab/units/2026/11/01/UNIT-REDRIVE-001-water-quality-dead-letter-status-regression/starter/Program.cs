using ReplayLab;

var projection = new StatusProjection();
var deliveries = new[]
{
    new StatusSnapshot("evt-01", "W-17", 1, "OPEN"),
    new StatusSnapshot("evt-03", "W-17", 3, "CLOSED"),
    new StatusSnapshot("evt-02", "W-17", 2, "SUSPENDED"),
    new StatusSnapshot("evt-03", "W-17", 3, "CLOSED")
};
foreach (var message in deliveries)
{
    var outcome = projection.Apply(message);
    var view = projection.Get(message.SiteId);
    Console.WriteLine($"delivery={message.EventId} version={message.Version} outcome={outcome} public={view?.Status} publicVersion={view?.Version} applied={projection.AppliedCount}");
}
