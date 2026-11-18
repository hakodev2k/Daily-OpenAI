using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

var cases = new[] { "A-101", "B-202", "C-303", "D-404" };
var uploader = new ReportUploader();
var report = await uploader.UploadBatchAsync(cases);
Console.WriteLine($"accepted={report.Accepted.Count}; rejected={report.Rejected.Count}");
foreach (var value in report.Accepted) Console.WriteLine($"ACCEPT {value}");
foreach (var value in report.Rejected) Console.WriteLine($"REJECT {value}");

public sealed record BatchReport(List<string> Accepted, List<string> Rejected);
public sealed class ReportUploader
{
    public async Task<BatchReport> UploadBatchAsync(IEnumerable<string> ids)
    {
        var batchIds = ids.ToArray();
        var tasks = batchIds.Select(UploadOneAsync).ToArray();
        try { await Task.WhenAll(tasks); }
        catch { /* Inspect each completed task below; never infer all failed. */ }
        var accepted = new List<string>();
        var rejected = new List<string>();
        foreach (var pair in batchIds.Zip(tasks))
        {
            if (pair.Second.IsCompletedSuccessfully) accepted.Add(pair.Second.Result);
            else if (pair.Second.IsFaulted) rejected.Add(pair.First);
            else if (pair.Second.IsCanceled) rejected.Add(pair.First);
        }
        return new BatchReport(accepted, rejected);
    }

    private static async Task<string> UploadOneAsync(string id)
    {
        await Task.Delay(id == "C-303" ? 40 : 10);
        if (id is "B-202" or "C-303") throw new InvalidOperationException($"partner rejected {id}");
        return id;
    }
}
