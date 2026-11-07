using System.Text;
public static class GateSchedulePublisher
{
    public static async Task PublishAsync(string destinationPath, string jsonSnapshot, Func<Task> atCheckpoint)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
        byte[] bytes = Encoding.UTF8.GetBytes(jsonSnapshot);
        await using var output = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
        int n = bytes.Length / 2;
        await output.WriteAsync(bytes.AsMemory(0, n));
        await output.FlushAsync();
        await atCheckpoint();
        await output.WriteAsync(bytes.AsMemory(n));
        await output.FlushAsync();
    }
}
