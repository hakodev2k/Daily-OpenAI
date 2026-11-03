using System;
using System.Collections.Generic;
using System.Linq;

record Alert(string Id, string Site, int Sequence, bool PermanentFailure = false, int TransientFailuresBeforeSuccess = 0);
sealed class PermanentGatewayException(string message) : Exception(message) { }
sealed class TransientGatewayException(string message) : Exception(message) { }

sealed class SimulatedGateway
{
    public Dictionary<string, int> Attempts { get; } = new(StringComparer.Ordinal);

    public void Send(Alert alert)
    {
        int attempt = Attempts.GetValueOrDefault(alert.Id) + 1;
        Attempts[alert.Id] = attempt;
        if (alert.PermanentFailure)
            throw new PermanentGatewayException($"recipient rejected for {alert.Id}");
        if (attempt <= alert.TransientFailuresBeforeSuccess)
            throw new TransientGatewayException($"temporary gateway 503 for {alert.Id}");
    }
}

sealed class Envelope(Alert alert)
{
    public Alert Alert { get; } = alert;
    public int Attempts { get; set; }
    public int NextTick { get; set; }
}

sealed class AlertProcessor
{
    private readonly List<Envelope> _pending;
    private readonly SimulatedGateway _gateway;
    public List<Alert> Delivered { get; } = [];
    public List<Alert> DeadLetters { get; } = [];
    public List<string> Timeline { get; } = [];

    public AlertProcessor(IEnumerable<Alert> alerts, SimulatedGateway gateway)
    {
        _pending = alerts.Select(a => new Envelope(a)).ToList();
        _gateway = gateway;
    }

    public void Process(int tickBudget)
    {
        for (int tick = 0; tick < tickBudget && _pending.Count > 0; tick++)
        {
            Envelope current = _pending[0];
            current.Attempts++;
            try
            {
                _gateway.Send(current.Alert);
                Delivered.Add(current.Alert);
                Timeline.Add($"tick={tick} delivered={current.Alert.Id}");
                _pending.RemoveAt(0);
            }
            catch (Exception ex)
            {
                Timeline.Add($"tick={tick} retry={current.Alert.Id} attempt={current.Attempts} error={ex.GetType().Name}");
            }
        }
    }

}

static class Program
{
    static Alert[] MainBatch() =>
    [
        new("P1", "site-a", 1, PermanentFailure: true),
        new("A2", "site-a", 2),
        new("B1", "site-b", 1),
        new("T1", "site-c", 1, TransientFailuresBeforeSuccess: 1),
        new("C2", "site-c", 2)
    ];

    static (AlertProcessor Processor, SimulatedGateway Gateway) Execute(IEnumerable<Alert> alerts, int ticks)
    {
        var gateway = new SimulatedGateway();
        var processor = new AlertProcessor(alerts, gateway);
        processor.Process(ticks);
        return (processor, gateway);
    }

    static bool VerifyPrimary()
    {
        var (processor, gateway) = Execute(MainBatch(), 12);
        return string.Join(",", processor.Delivered.Select(a => a.Id)) == "A2,B1,T1,C2"
            && string.Join(",", processor.DeadLetters.Select(a => a.Id)) == "P1"
            && gateway.Attempts.GetValueOrDefault("P1") == 1
            && gateway.Attempts.GetValueOrDefault("T1") == 2;
    }

    static bool VerifyExhaustionAndFairness()
    {
        Alert[] alerts =
        [
            new("X1", "site-d", 1, TransientFailuresBeforeSuccess: 99),
            new("Y1", "site-e", 1)
        ];
        var (processor, gateway) = Execute(alerts, 12);
        return string.Join(",", processor.Delivered.Select(a => a.Id)) == "Y1"
            && string.Join(",", processor.DeadLetters.Select(a => a.Id)) == "X1"
            && gateway.Attempts.GetValueOrDefault("X1") == 3;
    }

    static bool VerifyHealthyOnly()
    {
        var (processor, gateway) = Execute([new("Z1", "site-z", 1)], 2);
        return processor.Delivered.Count == 1
            && processor.Delivered[0].Id == "Z1"
            && processor.DeadLetters.Count == 0
            && gateway.Attempts.GetValueOrDefault("Z1") == 1;
    }

    static int Main(string[] args)
    {
        string mode = args.Length > 0 ? args[0] : "--demo";
        if (mode == "--verify")
        {
            bool a = VerifyPrimary();
            bool b = VerifyExhaustionAndFairness();
            bool c = VerifyHealthyOnly();
            Console.WriteLine($"primary={a}; exhaustion-and-fairness={b}; healthy-only={c}");
            return a && b && c ? 0 : 1;
        }

        var (processor, gateway) = Execute(MainBatch(), 12);
        foreach (string line in processor.Timeline) Console.WriteLine(line);
        Console.WriteLine($"delivered=[{string.Join(",", processor.Delivered.Select(a => a.Id))}]");
        Console.WriteLine($"deadLetters=[{string.Join(",", processor.DeadLetters.Select(a => a.Id))}]");
        Console.WriteLine($"P1-attempts={gateway.Attempts.GetValueOrDefault("P1")}");

        if (mode == "--reproduce")
        {
            bool symptom = processor.Delivered.Count == 0
                && processor.DeadLetters.Count == 0
                && gateway.Attempts.GetValueOrDefault("P1") == 12;
            Console.WriteLine($"starter-symptom-reproduced={symptom}");
            return symptom ? 0 : 1;
        }
        return mode == "--demo" ? 0 : 2;
    }
}
