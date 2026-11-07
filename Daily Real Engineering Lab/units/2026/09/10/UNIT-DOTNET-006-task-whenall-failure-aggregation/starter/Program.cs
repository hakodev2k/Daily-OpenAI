using System.Collections.Concurrent;

var operations = new[]
{
    RunDependencyAsync("TaxApi", shouldFail: true, delayMs: 80),
    RunDependencyAsync("BenefitsApi", shouldFail: true, delayMs: 120),
    RunDependencyAsync("LedgerApi", shouldFail: false, delayMs: 60)
};

var reportedFailures = new ConcurrentBag<string>();
var batchTask = Task.WhenAll(operations);

try
{
    await batchTask;
}
catch (Exception ex)
{
    // Investigation note:
    // Is the exception observed here guaranteed to represent every failed operation?
    reportedFailures.Add(ex.Message);
}

var faultedTasks = operations.Count(task => task.IsFaulted);
var successfulTasks = operations.Count(task => task.Status == TaskStatus.RanToCompletion);

Console.WriteLine($"FAULTED_TASKS={faultedTasks}");
Console.WriteLine($"SUCCESSFUL_TASKS={successfulTasks}");
Console.WriteLine($"REPORTED_FAILURES={reportedFailures.Count}");

foreach (var failure in reportedFailures.OrderBy(x => x, StringComparer.Ordinal))
{
    Console.WriteLine($"REPORTED={failure}");
}

if (faultedTasks == 2 && reportedFailures.Count == 1)
{
    Console.WriteLine("EXPECTED_FAILURE: only 1 of 2 failures reported");
    return;
}

if (faultedTasks == 2 && reportedFailures.Count == 2)
{
    Console.WriteLine("FIX_VERIFIED: all dependency failures reported");
    return;
}

Console.WriteLine("UNEXPECTED_RESULT");
Environment.ExitCode = 2;

static async Task<string> RunDependencyAsync(string name, bool shouldFail, int delayMs)
{
    await Task.Delay(delayMs);

    if (shouldFail)
    {
        throw new InvalidOperationException($"{name}: unavailable");
    }

    Console.WriteLine($"SUCCESS={name}");
    return name;
}
