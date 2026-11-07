record Ticket(string Id, string Text);

sealed class SearchIndexSimulator
{
    private readonly Dictionary<string, Ticket> committed = new();
    private readonly Dictionary<string, Ticket> searchable = new();

    public void Index(Ticket ticket) => committed[ticket.Id] = ticket;
    public Ticket? Get(string id) => committed.GetValueOrDefault(id);
    public IReadOnlyList<Ticket> Search(string text) => searchable.Values.Where(x => x.Text.Contains(text, StringComparison.OrdinalIgnoreCase)).ToList();
    public void Refresh() { foreach (var item in committed) searchable[item.Key] = item.Value; }
}

static class TicketWorkflow
{
    public static IReadOnlyList<Ticket> CreateThenSearch(SearchIndexSimulator index, Ticket ticket)
    {
        index.Index(ticket);
        // Investigation note: what visibility guarantee does this workflow actually require?
        return index.Search(ticket.Id);
    }
}

var mode = args.FirstOrDefault() ?? "demo";
var index = new SearchIndexSimulator();
var ticket = new Ticket("INC-1042", "INC-1042 payment callback investigation");

if (mode == "reproduce")
{
    var result = TicketWorkflow.CreateThenSearch(index, ticket);
    Console.WriteLine($"writeLookup={(index.Get(ticket.Id) is null ? "missing" : "present")}");
    Console.WriteLine($"immediateSearchCount={result.Count}");
    index.Refresh();
    Console.WriteLine($"afterVisibilityCycle={index.Search(ticket.Id).Count}");
    if (result.Count == 0 && index.Search(ticket.Id).Count == 1) { Console.Error.WriteLine("REPRODUCED: acknowledged write was not immediately searchable."); Environment.Exit(2); }
    Environment.Exit(1);
}

if (mode == "verify")
{
    var result = TicketWorkflow.CreateThenSearch(index, ticket);
    if (result.Any(x => x.Id == ticket.Id)) { Console.WriteLine("VERIFY PASSED"); Environment.Exit(0); }
    Console.Error.WriteLine("VERIFY FAILED: workflow visibility contract not satisfied."); Environment.Exit(3);
}

Console.WriteLine($"results={TicketWorkflow.CreateThenSearch(index, ticket).Count}");