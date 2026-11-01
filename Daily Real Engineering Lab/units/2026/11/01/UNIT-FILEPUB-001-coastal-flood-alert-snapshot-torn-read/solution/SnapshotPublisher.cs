using System.Text;

namespace CoastalAlertLab;

public sealed class SnapshotPublisher
{
    public void Publish(string targetPath, string json, Action? beforeCommit = null)
    {
        string fullPath = Path.GetFullPath(targetPath);
        string directory = Path.GetDirectoryName(fullPath)
            ?? throw new InvalidOperationException("Snapshot directory unavailable.");
        string temp = Path.Combine(directory,
            "." + Path.GetFileName(fullPath) + "." + Guid.NewGuid().ToString("N") + ".tmp");

        try
        {
            byte[] bytes = new UTF8Encoding(false).GetBytes(json);
            using (var stream = new FileStream(temp, FileMode.CreateNew,
                       FileAccess.Write, FileShare.None))
            {
                stream.Write(bytes);
                stream.Flush(flushToDisk: true);
            }
            beforeCommit?.Invoke();
            File.Move(temp, fullPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temp)) File.Delete(temp);
        }
    }
}
