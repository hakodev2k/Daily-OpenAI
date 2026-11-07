using System.Text;
namespace Lab;
public static class Utf8LineReader
{
    public static IReadOnlyList<string> ReadLines(IEnumerable<byte[]> frames)
    {
        var decoder = new UTF8Encoding(false, true).GetDecoder();
        var result = new List<string>();
        var buffer = new StringBuilder();
        void Append(ReadOnlySpan<char> text)
        {
            foreach (char c in text)
            {
                if (c == '\n')
                {
                    result.Add(buffer.ToString().TrimEnd('\r'));
                    buffer.Clear();
                }
                else buffer.Append(c);
            }
        }
        foreach (var frame in frames)
        {
            char[] chars = new char[frame.Length + 4];
            int count = decoder.GetChars(frame, 0, frame.Length, chars, 0, flush: false);
            Append(chars.AsSpan(0, count));
        }
        char[] tail = new char[4];
        int finalCount = decoder.GetChars(Array.Empty<byte>(), 0, 0, tail, 0, flush: true);
        Append(tail.AsSpan(0, finalCount));
        if (buffer.Length != 0) throw new InvalidDataException("Unterminated JSONL record.");
        return result;
    }
}
