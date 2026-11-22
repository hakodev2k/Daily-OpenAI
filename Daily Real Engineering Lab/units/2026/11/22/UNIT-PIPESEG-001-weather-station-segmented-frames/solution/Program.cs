using System.Buffers;
using System.Globalization;
using System.Text;

public sealed record Reading(string StationId, int Celsius);

public sealed class BufferPart : ReadOnlySequenceSegment<byte>
{
    public BufferPart(ReadOnlyMemory<byte> memory) { Memory = memory; }
    public BufferPart Append(ReadOnlyMemory<byte> memory)
    {
        var next = new BufferPart(memory) { RunningIndex = RunningIndex + Memory.Length };
        Next = next;
        return next;
    }
}

public static class Fixtures
{
    public static ReadOnlySequence<byte> Frame(params int[] lengths)
    {
        var bytes = Encoding.UTF8.GetBytes("S01|24\nS02|27\nS03|29\n");
        if (lengths.Length == 0) return new ReadOnlySequence<byte>(bytes);
        if (lengths.Any(n => n <= 0) || lengths.Sum() != bytes.Length)
            throw new ArgumentException("Segment lengths must sum to the frame length.");
        var offset = 0;
        var first = new BufferPart(bytes.AsMemory(offset, lengths[0]));
        var last = first;
        offset += lengths[0];
        foreach (var length in lengths.Skip(1))
        {
            last = last.Append(bytes.AsMemory(offset, length));
            offset += length;
        }
        return new ReadOnlySequence<byte>(first, 0, last, last.Memory.Length);
    }

    public static void Check(string name, ReadOnlySequence<byte> frame)
    {
        var actual = TelemetryParser.Decode(frame);
        var expected = new[] { new Reading("S01", 24), new Reading("S02", 27), new Reading("S03", 29) };
        if (!actual.SequenceEqual(expected))
            throw new InvalidOperationException($"{name}: expected {expected.Length} readings, got {actual.Count}: {string.Join(",", actual.Select(x => x.StationId))}");
        Console.WriteLine($"PASS {name}: {actual.Count} readings");
    }
}

public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length != 1 || (args[0] != "reproduce" && args[0] != "verify"))
        {
            Console.Error.WriteLine("Usage: dotnet run --project starter -- reproduce|verify");
            return 2;
        }

        var bytes = Encoding.UTF8.GetByteCount("S01|24\nS02|27\nS03|29\n");
        var segmented = Fixtures.Frame(5, 6, bytes - 11);
        if (args[0] == "reproduce")
        {
            try
            {
                Fixtures.Check("segmented", segmented);
                Console.Error.WriteLine("NOT REPRODUCED: original failure no longer present.");
                return 1;
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("REPRODUCED: " + ex.Message);
                return 0;
            }
        }

        try
        {
            Fixtures.Check("single-segment", Fixtures.Frame());
            Fixtures.Check("multi-segment", segmented);
            Fixtures.Check("byte-at-a-time", Fixtures.Frame(Enumerable.Repeat(1, bytes).ToArray()));
            Console.WriteLine("VERIFIED: all segmentation shapes preserve records and values.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("VERIFY FAILED: " + ex.Message);
            return 1;
        }
    }
}

public static class TelemetryParser
{
    public static List<Reading> Decode(ReadOnlySequence<byte> payload)
    {
        var text = Encoding.UTF8.GetString(payload.ToArray());
        return text.Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line =>
            {
                var parts = line.Split('|');
                if (parts.Length != 2 || !int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var temp))
                    throw new FormatException("Invalid telemetry frame.");
                return new Reading(parts[0], temp);
            })
            .ToList();
    }
}
