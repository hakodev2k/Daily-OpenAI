sealed record Node(string OrderId, int Version);
sealed class Dashboard
{
    private int _version = 1;
    public Node Resolve(string orderId) => new(orderId, _version);
    public void Refresh() => _version++;
    public bool Approve(Node node) => node.Version == _version && node.OrderId == "ORD-42";
}

static class OrderTest
{
    public static bool Run(Dashboard page)
    {
        // Investigation note: what assumptions does the test make about target lifetime?
        var approveTarget = page.Resolve("ORD-42");
        page.Refresh();
        return page.Approve(approveTarget);
    }
}

var mode = args.FirstOrDefault() ?? "run";
if (mode == "reproduce")
{
    var ok = OrderTest.Run(new Dashboard());
    Console.WriteLine($"approve-result={ok}");
    if (!ok) { Console.Error.WriteLine("REPRODUCED: action target became invalid after UI refresh."); return 2; }
    return 1;
}
if (mode == "verify")
{
    for (var i = 0; i < 20; i++)
    {
        if (!OrderTest.Run(new Dashboard())) { Console.Error.WriteLine($"VERIFY FAILED at cycle {i + 1}"); return 3; }
    }
    Console.WriteLine("VERIFY PASSED"); return 0;
}
Console.WriteLine(OrderTest.Run(new Dashboard()));
return 0;