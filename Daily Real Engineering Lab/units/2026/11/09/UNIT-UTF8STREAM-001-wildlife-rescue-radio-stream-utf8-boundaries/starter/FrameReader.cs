using System.Text;

namespace RadioGateway;

public static class FrameReader
{
    // Nhận các mảnh byte từ gateway; chunk boundaries do tầng vận chuyển quyết định.
    public static string ReadMessage(IEnumerable<byte[]> chunks)
    {
        var message = new StringBuilder();
        foreach (var chunk in chunks)
        {
            message.Append(Encoding.UTF8.GetString(chunk));
        }
        return message.ToString();
    }
}
