using System.Globalization;

var csv = args.Length > 0 ? args[0] : "200,1200,1500";
var source = new Queue<Payment>(
    csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
       .Select((value, index) =>
           new Payment($"P{index + 1:000}", decimal.Parse(value, CultureInfo.InvariantCulture))));

var payments = ReadPayments(source).ToList();

var total = payments.Sum(payment => payment.Amount);
var highRiskIds = payments
    .Where(payment => payment.Amount >= 1000m)
    .Select(payment => payment.Id)
    .ToArray();

Console.WriteLine($"Total={total.ToString(CultureInfo.InvariantCulture)}");
Console.WriteLine($"HighRiskCount={highRiskIds.Length}");
Console.WriteLine($"HighRiskIds={string.Join(',', highRiskIds)}");

static IEnumerable<Payment> ReadPayments(Queue<Payment> source)
{
    while (source.TryDequeue(out var payment))
    {
        Console.Error.WriteLine($"read:{payment.Id}");
        yield return payment;
    }
}

internal sealed record Payment(string Id, decimal Amount);
