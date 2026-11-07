using System.Text;

namespace CoastalAlertLab;

public sealed class SnapshotPublisher
{
    // Hook dùng để mô phỏng lịch chạy của reader và writer.
    public void Publish(string targetPath, string json, Action? beforeCommit = null)
    {
        byte[] bytes = new UTF8Encoding(false).GetBytes(json);
        using var stream = new FileStream(
            targetPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        beforeCommit?.Invoke();
        stream.Write(bytes);
        stream.Flush(flushToDisk: true);
    }
}
