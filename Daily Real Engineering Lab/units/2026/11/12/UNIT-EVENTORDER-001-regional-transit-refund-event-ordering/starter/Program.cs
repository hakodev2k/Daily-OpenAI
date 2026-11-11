using System;
using System.Collections.Generic;
using System.Linq;
record TicketEvent(string TicketId, long Sequence, string Status);
record TicketState(long Sequence, string Status);
sealed class TicketProjection
{
    private readonly Dictionary<string, TicketState> _states = new();
    public void Apply(TicketEvent evt)
    {
        // Legacy consumer: process incoming notifications.
        _states[evt.TicketId] = new TicketState(evt.Sequence, evt.Status);
    }
    public TicketState Get(string id) => _states[id];
}
static class Program
{
    static int Main(string[] args)
    {
        var events = new[] {
            new TicketEvent("T-731", 1, "Issued"),
            new TicketEvent("T-731", 3, "Refunded"),
            new TicketEvent("T-731", 2, "Paid"),
            new TicketEvent("T-731", 3, "Refunded")
        };
        var p = new TicketProjection();
        foreach (var e in events.Take(3)) p.Apply(e);
        var observed = p.Get("T-731");
        Console.WriteLine($"Observed: {observed.Status}/{observed.Sequence}");
        if (args.Contains("--reproduce"))
        {
            if (observed.Status != "Refunded") { Console.WriteLine("REPRODUCED"); return 0; }
            Console.Error.WriteLine("NOT REPRODUCED"); return 1;
        }
        var cases = new[] {
            new[] {events[0], events[1], events[2]},
            new[] {events[2], events[0], events[1]},
            new[] {events[0], events[1], events[3], events[2]}
        };
        for (int i = 0; i < cases.Length; i++)
        {
            var projection = new TicketProjection();
            foreach (var e in cases[i]) projection.Apply(e);
            var result = projection.Get("T-731");
            if (result.Sequence != 3 || result.Status != "Refunded")
            {
                Console.Error.WriteLine($"FAIL case {i+1}: {result.Status}/{result.Sequence}");
                return 1;
            }
        }
        Console.WriteLine("PASS: three delivery permutations");
        return 0;
    }
}
