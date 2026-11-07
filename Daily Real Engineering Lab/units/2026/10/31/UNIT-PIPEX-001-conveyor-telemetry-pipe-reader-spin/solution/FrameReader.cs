using System.Buffers;
using System.IO.Pipelines;
using System.Text;

namespace ConveyorTelemetry;

public sealed class FrameReader
{
    // Returns one newline-terminated UTF-8 frame, without the newline.
    public async Task<string> ReadFrameAsync(
        PipeReader reader, Action onIncompleteFrame, CancellationToken cancellationToken = default)
    {
        while (true)
        {
            ReadResult result = await reader.ReadAsync(cancellationToken);
            ReadOnlySequence<byte> buffer = result.Buffer;
            SequencePosition? delimiter = buffer.PositionOf((byte)'\n');

            if (delimiter is { } position)
            {
                string frame = Encoding.UTF8.GetString(buffer.Slice(0, position).ToArray());
                reader.AdvanceTo(buffer.GetPosition(1, position));
                return frame;
            }

            if (result.IsCompleted)
            {
                reader.AdvanceTo(buffer.End);
                throw new InvalidDataException("Stream ended with an incomplete telemetry frame.");
            }

            onIncompleteFrame();
            reader.AdvanceTo(buffer.Start, buffer.End);
        }
    }
}
