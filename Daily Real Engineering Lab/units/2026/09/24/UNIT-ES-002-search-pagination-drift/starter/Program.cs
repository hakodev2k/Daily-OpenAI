record Ticket(int Id, int UpdatedRank);

var tickets = Enumerable.Range(1, 25).Select(i => new Ticket(i, 1000 - i)).ToList();

List<int> Page(int page, int size) => tickets
    .OrderByDescending(x => x.UpdatedRank)
    .Skip((page - 1) * size)
    .Take(size)
    .Select(x => x.Id)
    .ToList();

var page1 = Page(1, 10);
Console.WriteLine($"page1={string.Join(',', page1)}");

// Một ticket vừa được cập nhật trong lúc user chuyển sang trang kế tiếp.
var changed = tickets.Single(x => x.Id == 15);
tickets[tickets.IndexOf(changed)] = changed with { UpdatedRank = 2000 };
Console.WriteLine("mutation=ticket-15-updated");

var page2 = Page(2, 10);
Console.WriteLine($"page2={string.Join(',', page2)}");

var duplicates = page1.Intersect(page2).ToArray();
Console.WriteLine($"duplicates={string.Join(',', duplicates)}");
Console.WriteLine($"uniqueCount={page1.Concat(page2).Distinct().Count()}");