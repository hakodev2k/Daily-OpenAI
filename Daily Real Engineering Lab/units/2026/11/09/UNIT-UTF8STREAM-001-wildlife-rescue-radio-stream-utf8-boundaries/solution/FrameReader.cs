using System.Text;

namespace RadioGateway;

public static class FrameReader
{
    public static string ReadMessage(IEnumerable<byte[]> chunks)
    {
        var decoder = Encoding.UTF8.GetDecoder();
        var result = new StringBuilder();
        var chars = new char[4096];
        foreach (var chunk in chunks)
        {
            var offset = 0;
            while (offset < chunk.Length)
            {
                decoder.Convert(chunk, offset, chunk.Length - offset,
                    chars, 0, chars.Length, flush: false,
                    out var bytesUsed, out var charsUsed, out _);
                result.Append(chars, 0, charsUsed);
                offset += bytesUsed;
                if (bytesUsed == 0 && charsUsed == 0)
                    throw new InvalidOperationException("Decoder did not make progress.");
            }
        }
        decoder.Convert(Array.Empty<byte>(), 0, 0, chars, 0, chars.Length,
            flush: true, out _, out var finalChars, out _);
        result.Append(chars, 0, finalChars);
        return result.ToString();
    }
}
