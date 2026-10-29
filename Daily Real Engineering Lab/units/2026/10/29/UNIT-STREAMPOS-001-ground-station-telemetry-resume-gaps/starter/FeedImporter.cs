using System.Globalization;
using System.Text;

namespace StreamCheckpointLab;

public sealed class FeedImporter
{
    // Feed contract: append-only UTF-8 text; one non-empty event per line.
    // The cursor file survives worker restarts.
    public IReadOnlyList<string> Import(string feedPath, string checkpointPath, int maxRecords)
    {
        if (maxRecords <= 0) throw new ArgumentOutOfRangeException(nameof(maxRecords));

        long cursor = File.Exists(checkpointPath)
            ? long.Parse(File.ReadAllText(checkpointPath), CultureInfo.InvariantCulture)
            : 0;

        using var stream = File.OpenRead(feedPath);
        stream.Seek(cursor, SeekOrigin.Begin);
        using var reader = new StreamReader(
            stream, new UTF8Encoding(false, true),
            detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);

        var accepted = new List<string>();
        while (accepted.Count < maxRecords)
        {
            var line = reader.ReadLine();
            if (line is null) break;
            accepted.Add(line);
            File.WriteAllText(checkpointPath,
                stream.Position.ToString(CultureInfo.InvariantCulture));
        }

        return accepted;
    }
}
