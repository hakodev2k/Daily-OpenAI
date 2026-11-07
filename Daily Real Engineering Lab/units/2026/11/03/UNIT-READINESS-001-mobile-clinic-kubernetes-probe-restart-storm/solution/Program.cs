using System;
using System.Collections.Generic;
record Tick(int Minute, bool DatabaseAvailable, bool ProcessAlive = true);
record Result(int Restarts, int Rejected, int Accepted, int LostInFlight);
static class Program
{
    static Result Simulate(IEnumerable<Tick> ticks)
    {
        int restarts=0,rejected=0,accepted=0,lost=0,inflight=3;
        foreach(var tick in ticks)
        {
            // Liveness tests the process itself; downstream outage is not process death.
            bool liveness=tick.ProcessAlive;
            // Readiness determines whether new traffic is safe to accept.
            bool readiness=liveness && tick.DatabaseAvailable;
            if(!liveness) { restarts++; lost+=inflight; inflight=0; }
            if(!readiness) rejected++; else { accepted++; inflight=3; }
            Console.WriteLine($"minute={tick.Minute} db={tick.DatabaseAvailable} live={liveness} ready={readiness} restarts={restarts} lost={lost}");
        }
        return new(restarts,rejected,accepted,lost);
    }
    static int Main()
    {
        var ticks=new[]{new Tick(0,true),new Tick(1,false),new Tick(2,false),new Tick(3,true)};
        var result=Simulate(ticks);
        Console.WriteLine($"SUMMARY restarts={result.Restarts} rejected={result.Rejected} accepted={result.Accepted} lost={result.LostInFlight}");
        return result.Restarts==0 && result.Rejected==2 && result.Accepted==2 && result.LostInFlight==0 ? 0 : 1;
    }
}
