using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MealReportLab;

public sealed record SupplierResult(string Supplier, int Items);

public sealed record ReportOutcome(bool Published, int TotalItems, IReadOnlyList<string> Diagnostics);

public sealed class SupplierFeedException : Exception
{
    public string Supplier { get; }
    public string ErrorCode { get; }

    public SupplierFeedException(string supplier, string errorCode)
        : base($"{supplier}:{errorCode}")
    {
        Supplier = supplier;
        ErrorCode = errorCode;
    }
}

public static class SupplierReport
{
    public static async Task<ReportOutcome> BuildAsync(
        IReadOnlyList<Func<Task<SupplierResult>>> sources,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var pending = sources.Select(source => source()).ToArray();

        try
        {
            var results = await Task.WhenAll(pending).ConfigureAwait(false);
            return new ReportOutcome(
                Published: true,
                TotalItems: results.Sum(result => result.Items),
                Diagnostics: Array.Empty<string>());
        }
        catch (Exception exception)
        {
            return new ReportOutcome(
                Published: false,
                TotalItems: 0,
                Diagnostics: new[] { exception.Message });
        }
    }
}
