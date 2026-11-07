using System.Text;

namespace ChunkedTextLab;

public static class ChunkedNoteDecoder
{
    public static string Decode(IEnumerable<ReadOnlyMemory<byte>> fragments)
    {
        // Strict UTF-8: reject malformed sequences instead of inserting U+FFFD.
        Decoder decoder = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
            .GetDecoder();
        var result = new StringBuilder();

        foreach (var fragment in fragments)
        {
            // The decoder preserves incomplete code points between fragments.
            char[] buffer = new char[Encoding.UTF8.GetMaxCharCount(fragment.Length)];
            int count = decoder.GetChars(fragment.Span, buffer.AsSpan(), flush: false);
            result.Append(buffer, 0, count);
        }

        // End-of-message is the only point at which an incomplete tail is invalid.
        char[] tail = new char[4];
        int tailCount = decoder.GetChars(ReadOnlySpan<byte>.Empty, tail.AsSpan(), flush: true);
        result.Append(tail, 0, tailCount);
        return result.ToString();
    }
}
