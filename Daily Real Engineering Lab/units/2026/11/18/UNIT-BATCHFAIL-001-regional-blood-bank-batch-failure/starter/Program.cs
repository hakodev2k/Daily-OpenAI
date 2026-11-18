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
        var tasks = ids.Select(UploadOneAsync).ToArray();
        try
        {
            var accepted = await Task.WhenAll(tasks);
            return new BatchReport(accepted.ToList(), new List<string>());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"batch-error={ex.GetType().Name}:{ex.Message}");
            return new BatchReport(new List<string>(), new List<string> { "batch" });
        }
    }

    private static async Task<string> UploadOneAsync(string id)
    {
        await Task.Delay(id == "C-303" ? 40 : 10);
        if (id is "B-202" or "C-303") throw new InvalidOperationException($"partner rejected {id}");
        return id;
    }
}
