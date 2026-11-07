using System;
using System.Collections.Generic;
using System.Text;

return LabRunner.Execute(args);

internal static class LabRunner
{
    public static int Execute(string[] args)
    {
        string mode = args.Length == 0 ? "run" : args[0].ToLowerInvariant();
        return mode switch
        {
            "run" => Run(),
            "reproduce" => Reproduce(),
            "verify" => Verify(),
            _ => Unknown(mode)
        };
    }

    private static int Unknown(string mode)
    {
        Console.Error.WriteLine($"Unknown mode: {mode}. Use run, reproduce, or verify.");
        return 2;
    }

    private static int Run()
    {
        var pipeline = new TelemetryPipeline(new OneSlotBufferPool());
        pipeline.Receive("sensor-A", "temperature:21.4");
        pipeline.Receive("sensor-B", "temperature:39.1");
        foreach (DecodedEvent item in pipeline.Drain())
            Console.WriteLine($"{item.SensorId} -> {item.Payload}");
        return 0;
    }

    private static int Reproduce()
    {
        var pipeline = new TelemetryPipeline(new OneSlotBufferPool());
        pipeline.Receive("sensor-A", "temperature:21.4");
        pipeline.Receive("sensor-B", "temperature:39.1");
        List<DecodedEvent> actual = pipeline.Drain();
        foreach (DecodedEvent item in actual)
            Console.WriteLine($"{item.SensorId} -> {item.Payload}");
        bool observed = actual.Count == 2
            && actual[0].Payload != "temperature:21.4"
            && actual[1].Payload == "temperature:39.1";
        Console.WriteLine(observed
            ? "EXPECTED SYMPTOM REPRODUCED: the first event changed after a later receive."
            : "SYMPTOM NOT REPRODUCED: inspect the starter and environment.");
        return observed ? 0 : 1;
    }

    private static int Verify()
    {
        (string Name, string[] Payloads)[] cases =
        {
            ("same-size-overwrite", new[] { "temperature:21.4", "temperature:39.1" }),
            ("different-size-overwrite", new[] { "a-long-sensor-payload-123456789", "ok" }),
            ("multiple-pending-events", new[] { "alpha", "bravo", "charlie", "delta" }),
            ("utf8-boundary", new[] { "nhiệt độ:25", "độ ẩm:80" })
        };
        int failures = 0;
        foreach (var test in cases)
        {
            var pipeline = new TelemetryPipeline(new OneSlotBufferPool());
            for (int i = 0; i < test.Payloads.Length; i++)
                pipeline.Receive($"sensor-{i}", test.Payloads[i]);
            List<DecodedEvent> actual = pipeline.Drain();
            bool pass = actual.Count == test.Payloads.Length;
            for (int i = 0; i < test.Payloads.Length && i < actual.Count; i++)
                pass &= actual[i].SensorId == $"sensor-{i}"
                    && actual[i].Payload == test.Payloads[i];
            Console.WriteLine($"{(pass ? "PASS" : "FAIL")} {test.Name}");
            if (!pass)
            {
                failures++;
                for (int i = 0; i < actual.Count; i++)
                    Console.WriteLine($"  actual[{i}]: {actual[i].SensorId} -> {actual[i].Payload}");
            }
        }
        Console.WriteLine(failures == 0 ? "VERIFICATION PASSED" : $"VERIFICATION FAILED: {failures} case(s)");
        return failures == 0 ? 0 : 1;
    }
}

internal sealed class OneSlotBufferPool
{
    private byte[]? _available;

    public byte[] Rent(int minimumLength)
    {
        if (_available is { } buffer && buffer.Length >= minimumLength)
        {
            _available = null;
            return buffer;
        }
        return new byte[Math.Max(128, minimumLength)];
    }

    public void Return(byte[] buffer) => _available = buffer;
}

internal sealed class TelemetryPipeline
{
    private readonly OneSlotBufferPool _pool;
    private readonly Queue<PendingEvent> _pending = new();

    public TelemetryPipeline(OneSlotBufferPool pool) => _pool = pool;

    public void Receive(string sensorId, string payload)
    {
        int byteCount = Encoding.UTF8.GetByteCount(payload);
        byte[] buffer = _pool.Rent(byteCount);
        try
        {
            int written = Encoding.UTF8.GetBytes(payload.AsSpan(), buffer.AsSpan());
            // Queue-owned snapshot survives reuse of the rented transport buffer.
            byte[] owned = buffer.AsSpan(0, written).ToArray();
            _pending.Enqueue(new PendingEvent(sensorId, owned));
        }
        finally
        {
            _pool.Return(buffer);
        }
    }

    public List<DecodedEvent> Drain()
    {
        var result = new List<DecodedEvent>();
        while (_pending.Count > 0)
        {
            PendingEvent item = _pending.Dequeue();
            result.Add(new DecodedEvent(item.SensorId, Encoding.UTF8.GetString(item.Payload.Span)));
        }
        return result;
    }
}

internal sealed record PendingEvent(string SensorId, ReadOnlyMemory<byte> Payload);
internal sealed record DecodedEvent(string SensorId, string Payload);
