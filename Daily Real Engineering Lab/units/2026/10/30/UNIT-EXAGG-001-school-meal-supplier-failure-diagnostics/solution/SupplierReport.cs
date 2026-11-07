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
        var combined = Task.WhenAll(pending);

        try
        {
            var results = await combined.ConfigureAwait(false);
            return new ReportOutcome(
                Published: true,
                TotalItems: results.Sum(result => result.Items),
                Diagnostics: Array.Empty<string>());
        }
        catch (OperationCanceledException) when (combined.IsCanceled)
        {
            throw;
        }
        catch (Exception)
        {
            var errors = combined.Exception?.Flatten().InnerExceptions
                ?? throw new InvalidOperationException("Faulted aggregate has no errors.");

            var diagnostics = errors
                .Select(error => error is SupplierFeedException feed
                    ? $"{feed.Supplier}:{feed.ErrorCode}"
                    : $"{error.GetType().Name}:{error.Message}")
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray();

            return new ReportOutcome(false, 0, diagnostics);
        }
    }
}
