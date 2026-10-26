public static class GateSchedulePublisher
{
    public static async Task PublishAsync(string destinationPath, string jsonSnapshot, Func<Task> atCheckpoint)
    {
        string dir = Path.GetDirectoryName(destinationPath)!;
        Directory.CreateDirectory(dir);
        string staging = Path.Combine(dir, "." + Path.GetFileName(destinationPath) + "." + Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            await File.WriteAllTextAsync(staging, jsonSnapshot);
            await atCheckpoint();
            File.Move(staging, destinationPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(staging)) File.Delete(staging);
        }
    }
}
