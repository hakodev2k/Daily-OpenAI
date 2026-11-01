using System.Text;

namespace ChunkedTextLab;

public static class ChunkedNoteDecoder
{
    // Gateway forwards payload fragments in the order they arrive.
    // The caller owns the fragment buffers for the duration of this call.
    public static string Decode(IEnumerable<ReadOnlyMemory<byte>> fragments)
    {
        var result = new StringBuilder();
        foreach (var fragment in fragments)
        {
            result.Append(Encoding.UTF8.GetString(fragment.Span));
        }

        return result.ToString();
    }
}
