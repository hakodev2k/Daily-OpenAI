using System.Globalization;

record Household(string Id, decimal Weight);
record Credit(string Id, decimal Amount);

static class Fixtures
{
    public static readonly Household[] Members = [new("H03", 1m), new("H01", 1m), new("H02", 1m)];
    public static void Check(Func<decimal, IReadOnlyList<Household>, IReadOnlyList<Credit>> allocate)
    {
        foreach (decimal total in new[] { 0.10m, 0.02m, 12.37m })
        {
            var a = allocate(total, Members);
            var b = allocate(total, Members.Reverse().ToArray());
            Assert(a.Count == 3, "Số người nhận không đúng");
            Assert(a.Sum(x => x.Amount) == total, $"Không bảo toàn tổng: {total} -> {a.Sum(x => x.Amount)}");
            Assert(a.All(x => x.Amount >= 0m && x.Amount * 100m == decimal.Truncate(x.Amount * 100m)), "Có số âm hoặc fraction của cent");
            Assert(a.OrderBy(x => x.Id).SequenceEqual(b.OrderBy(x => x.Id)), "Kết quả phụ thuộc thứ tự input");
        }
        var unequal = new[] { new Household("A", 5m), new Household("B", 3m), new Household("C", 2m) };
        var result = allocate(0.11m, unequal);
        Assert(result.Sum(x => x.Amount) == 0.11m, "Tổng lệch khi weight khác nhau");
        Assert(result.Single(x => x.Id == "A").Amount >= result.Single(x => x.Id == "C").Amount, "Không giữ tỷ lệ tương đối");
        Console.WriteLine("PASS: conservation, minor units, order invariance, weighted distribution");
    }
    static void Assert(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
}

static class CreditAllocator
{
    public static IReadOnlyList<Credit> Allocate(decimal total, IReadOnlyList<Household> members)
    {
        if (total < 0m || total * 100m != decimal.Truncate(total * 100m) || members.Count == 0 || members.Any(m => m.Weight <= 0m) || members.Select(m => m.Id).Distinct(StringComparer.Ordinal).Count() != members.Count)
            throw new ArgumentException("Invalid settlement");
        long cents = checked((long)(total * 100m));
        decimal weightTotal = members.Sum(m => m.Weight);
        var portions = members.Select(m => {
            decimal exact = cents * m.Weight / weightTotal;
            long floor = (long)decimal.Floor(exact);
            return new { m.Id, Floor = floor, Remainder = exact - floor };
        }).ToArray();
        long remaining = cents - portions.Sum(x => x.Floor);
        var winners = portions.OrderByDescending(x => x.Remainder).ThenBy(x => x.Id, StringComparer.Ordinal).Take(checked((int)remaining)).Select(x => x.Id).ToHashSet(StringComparer.Ordinal);
        return portions.Select(x => new Credit(x.Id, (x.Floor + (winners.Contains(x.Id) ? 1 : 0)) / 100m)).ToArray();
    }
}

try { Fixtures.Check(CreditAllocator.Allocate); }
catch (Exception ex) { Console.Error.WriteLine("FAIL: " + ex.Message); Environment.ExitCode = 1; }
