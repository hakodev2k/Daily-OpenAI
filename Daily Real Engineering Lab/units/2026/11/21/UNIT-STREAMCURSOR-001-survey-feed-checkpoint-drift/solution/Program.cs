using System.Text;
using System.Linq;

var root = Path.Combine(Path.GetTempPath(), "survey-checkpoint-lab-" + Environment.ProcessId);
Directory.CreateDirectory(root);
var source = Path.Combine(root, "feed.txt");
var cursor = Path.Combine(root, "cursor.txt");
File.WriteAllText(source, "A001,18.5\nA002,19.0\nA003,21.5\nA004,22.0\n", new UTF8Encoding(false));
File.WriteAllText(cursor, "0");
var first = CheckpointReader.ReadBatch(source, cursor, 1);
var second = CheckpointReader.ReadBatch(source, cursor, 3);
var all = first.Concat(second).ToArray();
Console.WriteLine("batch1=" + string.Join("|", first));
Console.WriteLine("batch2=" + string.Join("|", second));
Console.WriteLine("rows=" + all.Length + " unique=" + all.Distinct().Count());
Console.WriteLine("checkpoint=" + File.ReadAllText(cursor));
var expected = new[] { "A001,18.5", "A002,19.0", "A003,21.5", "A004,22.0" };
if (args.Contains("verify", StringComparer.OrdinalIgnoreCase))
{
    if (!all.SequenceEqual(expected))
    {
        Console.Error.WriteLine("FAIL: resumed batches must return all four records in order.");
        Environment.ExitCode = 1;
    }
    else Console.WriteLine("PASS: checkpoint resume preserves every record.");
}
else Console.WriteLine("OBSERVE: compare received records against the four-line source.");
public static class CheckpointReader
{
    public static List<string> ReadBatch(string source, string checkpoint, int maxRows)
    {
        using var stream = File.OpenRead(source);
        stream.Seek(long.Parse(File.ReadAllText(checkpoint)), SeekOrigin.Begin);
        var rows = new List<string>();
        for (int i = 0; i < maxRows; i++)
        {
            using var buffer = new MemoryStream();
            int next;
            while ((next = stream.ReadByte()) != -1 && next != 10)
                buffer.WriteByte((byte)next);
            if (next == -1 && buffer.Length == 0) break;
            var bytes = buffer.ToArray();
            if (bytes.Length > 0 && bytes[^1] == 13) Array.Resize(ref bytes, bytes.Length - 1);
            rows.Add(new UTF8Encoding(false, true).GetString(bytes));
        }
        File.WriteAllText(checkpoint, stream.Position.ToString(System.Globalization.CultureInfo.InvariantCulture));
        return rows;
    }
}
