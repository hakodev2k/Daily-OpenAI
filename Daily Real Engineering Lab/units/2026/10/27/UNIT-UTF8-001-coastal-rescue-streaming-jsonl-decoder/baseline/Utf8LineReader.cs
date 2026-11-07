using System.Text;
namespace Lab;
public static class Utf8LineReader
{
    public static IReadOnlyList<string> ReadLines(IEnumerable<byte[]> frames)
    {
        var result = new List<string>();
        var buffer = new StringBuilder();
        foreach (var frame in frames)
        {
            string text = Encoding.UTF8.GetString(frame);
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
        if (buffer.Length != 0) throw new InvalidDataException("Unterminated JSONL record.");
        return result;
    }
}
