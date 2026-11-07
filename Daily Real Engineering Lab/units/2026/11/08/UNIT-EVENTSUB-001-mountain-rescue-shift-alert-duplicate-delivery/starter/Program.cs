using System;
using System.Collections.Generic;

var hub = new AlertHub();
var coordinator = new ShiftCoordinator(hub);
var received = new List<string>();
foreach (var shift in new[] { "A", "B", "C" })
{
    coordinator.BeginShift(shift, received.Add);
    hub.Publish("ridge-weather-warning");
}
var expected = new[] { "A:ridge-weather-warning", "B:ridge-weather-warning", "C:ridge-weather-warning" };
Console.WriteLine("Expected: " + string.Join(" | ", expected));
Console.WriteLine("Actual:   " + string.Join(" | ", received));
Console.WriteLine($"Delivered={received.Count}; Expected={expected.Length}");
bool correct = received.Count == expected.Length;
for (int i = 0; i < Math.Min(received.Count, expected.Length); i++)
    correct &= received[i] == expected[i];
if (args.Length == 1 && args[0] == "--reproduce")
{
    if (correct) { Console.Error.WriteLine("Symptom absent: inspect starter baseline."); return 1; }
    Console.WriteLine("REPRODUCED: stale or duplicate deliveries observed.");
    return 0;
}
if (args.Length == 1 && args[0] == "--verify")
{
    if (!correct) { Console.Error.WriteLine("FAIL: delivery isolation contract violated."); return 1; }
    Console.WriteLine("PASS: exactly one current-shift delivery per publish.");
    return 0;
}
Console.WriteLine(correct ? "Contract passes" : "Contract fails");
return correct ? 0 : 1;

public sealed class AlertHub
{
    public event Action<string>? AlertRaised;
    public void Publish(string message) => AlertRaised?.Invoke(message);
}

public sealed class ShiftCoordinator
{
    private readonly AlertHub _hub;
    public ShiftCoordinator(AlertHub hub) => _hub = hub;
    public void BeginShift(string shiftId, Action<string> sink)
    {
        _hub.AlertRaised += message => sink($"{shiftId}:{message}");
    }
}
