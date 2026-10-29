using System.Globalization;
using System.Text;

namespace StreamCheckpointLab;

public sealed class FeedImporter
{
    // The persisted cursor represents a count of fully consumed logical lines,
    // not the position of the buffered underlying byte stream.
    public IReadOnlyList<string> Import(string feedPath, string checkpointPath, int maxRecords)
    {
        if (maxRecords <= 0) throw new ArgumentOutOfRangeException(nameof(maxRecords));

        long consumedLines = File.Exists(checkpointPath)
            ? long.Parse(File.ReadAllText(checkpointPath), CultureInfo.InvariantCulture)
            : 0;
        if (consumedLines < 0) throw new InvalidDataException("Negative checkpoint");

        using var stream = File.OpenRead(feedPath);
        using var reader = new StreamReader(
            stream, new UTF8Encoding(false, true),
            detectEncodingFromByteOrderMarks: false, bufferSize: 1024);

        for (long i = 0; i < consumedLines; i++)
        {
            if (reader.ReadLine() is null)
                throw new InvalidDataException("Feed truncated or checkpoint belongs to another file");
        }

        var accepted = new List<string>();
        while (accepted.Count < maxRecords)
        {
            var line = reader.ReadLine();
            if (line is null) break;
            accepted.Add(line);
            consumedLines++;
            File.WriteAllText(checkpointPath,
                consumedLines.ToString(CultureInfo.InvariantCulture));
        }

        return accepted;
    }
}
