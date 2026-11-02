public sealed class CertificateIndexer
{
    private readonly string _root;
    private readonly Dictionary<string, string> _index = new(StringComparer.OrdinalIgnoreCase);

    public CertificateIndexer(string root) => _root = root;

    public int NotificationsReceived { get; private set; }
    public int MaintenanceTicks { get; private set; }

    public void OnNotification(string fileName)
    {
        NotificationsReceived++;
        if (!fileName.EndsWith(".cert", StringComparison.OrdinalIgnoreCase))
            return;

        var path = Path.Combine(_root, fileName);
        if (File.Exists(path))
            _index[fileName] = File.ReadAllText(path).Trim();
        else
            _index.Remove(fileName);
    }

    public void MaintenanceTick()
    {
        MaintenanceTicks++;
        var disk = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var path in Directory.EnumerateFiles(_root, "*.cert", SearchOption.TopDirectoryOnly))
        {
            var name = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(name))
                disk[name] = File.ReadAllText(path).Trim();
        }

        foreach (var name in _index.Keys.ToArray())
            if (!disk.ContainsKey(name))
                _index.Remove(name);

        foreach (var pair in disk)
            _index[pair.Key] = pair.Value;
    }

    public IReadOnlyDictionary<string, string> Snapshot()
        => new Dictionary<string, string>(_index, StringComparer.OrdinalIgnoreCase);
}
