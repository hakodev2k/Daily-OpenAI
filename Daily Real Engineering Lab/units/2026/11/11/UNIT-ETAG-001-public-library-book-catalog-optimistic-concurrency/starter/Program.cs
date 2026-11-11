using System;

record Book(string Title, int Copies, int Revision);
sealed class Catalog
{
    private Book _book = new("Distributed Systems", 3, 1);
    public Book Read() => _book with { };
    public bool Save(Book incoming)
    {
        // Existing integration accepts full-record edits from both clients.
        _book = incoming with { Revision = _book.Revision + 1 };
        return true;
    }
}

var catalog = new Catalog();
var a = catalog.Read();
var b = catalog.Read();
var first = catalog.Save(a with { Copies = 4 });
var second = catalog.Save(b with { Title = "Distributed Systems, 2nd Edition" });
var final = catalog.Read();
Console.WriteLine($"first={first} second={second} title={final.Title} copies={final.Copies} revision={final.Revision}");
if (args.Contains("reproduce"))
{
    if (final.Copies != 3 || !first || !second) throw new Exception("Scenario changed: expected the original symptom.");
    Console.WriteLine("REPRODUCED: one accepted change is absent.");
}
else if (args.Contains("verify"))
{
    if (final.Copies != 4 || second || !first || final.Revision != 2)
        throw new Exception("VERIFY FAILED: stale edit must be rejected without overwriting first update.");
    Console.WriteLine("VERIFIED: stale edit rejected, first edit preserved.");
}
