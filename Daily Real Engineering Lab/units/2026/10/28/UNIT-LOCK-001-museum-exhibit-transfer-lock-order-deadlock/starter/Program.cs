using System.Diagnostics;
using System.Threading;
using ExhibitLedger;

var inventory = new GalleryInventory();
using var rendezvous = new Barrier(2);
Exception? failure = null;
var failureSync = new object();

void Transfer(string source, string destination)
{
    try
    {
        inventory.Transfer(source, destination, 1,
            () => rendezvous.SignalAndWait(TimeSpan.FromMilliseconds(450)));
    }
    catch (Exception ex)
    {
        lock (failureSync)
        {
            failure ??= ex;
        }
    }
}

var northToSouth = new Thread(() => Transfer("NORTH", "SOUTH"))
{
    IsBackground = true,
    Name = "north-to-south"
};
var southToNorth = new Thread(() => Transfer("SOUTH", "NORTH"))
{
    IsBackground = true,
    Name = "south-to-north"
};

var timer = Stopwatch.StartNew();
northToSouth.Start();
southToNorth.Start();

var budget = TimeSpan.FromSeconds(4);
var firstFinished = northToSouth.Join(budget);
var remaining = budget - timer.Elapsed;
var secondFinished = southToNorth.Join(remaining > TimeSpan.Zero ? remaining : TimeSpan.Zero);

if (!firstFinished || !secondFinished)
{
    Console.WriteLine("DEADLOCK_DETECTED: transfers did not finish within the fixture deadline.");
    return 2;
}

if (failure is not null)
{
    Console.WriteLine($"TRANSFER_FAILED: {failure.GetType().Name}: {failure.Message}");
    return 3;
}

var (north, south) = inventory.Snapshot();
if (north != 10 || south != 10 || north + south != 20)
{
    Console.WriteLine($"INVARIANT_FAILED: NORTH={north}; SOUTH={south}; TOTAL={north + south}");
    return 4;
}

Console.WriteLine($"PASS: both transfers completed; NORTH={north}; SOUTH={south}; TOTAL={north + south}");
return 0;
