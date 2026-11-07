var state = new OperationState();
var timeline = new List<string>();

try
{
    try
    {
        state.ActiveOperations++;
        timeline.Add($"work:active={state.ActiveOperations}");
        throw new InvalidOperationException("downstream failed");
    }
    finally
    {
        state.ActiveOperations--;
        timeline.Add($"cleanup:active={state.ActiveOperations}");
    }
}
catch (InvalidOperationException) when (ShouldHandle(state, timeline))
{
    timeline.Add($"caught:active={state.ActiveOperations}");
}

foreach (var item in timeline) Console.WriteLine(item);
var mode = args.FirstOrDefault() ?? "--run";
var policyIndex = timeline.FindIndex(x => x.StartsWith("policy:"));
var cleanupIndex = timeline.FindIndex(x => x.StartsWith("cleanup:"));
if (mode == "--reproduce") return policyIndex >= 0 && cleanupIndex >= 0 && policyIndex < cleanupIndex ? 0 : 1;
if (mode == "--verify")
{
    var ok = policyIndex < 0 || policyIndex > cleanupIndex;
    Console.WriteLine(ok ? "VERIFY PASS" : "VERIFY FAIL");
    return ok ? 0 : 1;
}
return 0;

static bool ShouldHandle(OperationState state, List<string> timeline)
{
    timeline.Add($"policy:active={state.ActiveOperations}");
    // Investigation note: is this state already stable when the filter executes?
    return state.ActiveOperations == 1;
}

sealed class OperationState { public int ActiveOperations { get; set; } }
