using System.Text.Json;

namespace CertificateDropLab;

public sealed record Certificate(string CertificateId, string DeviceId);

public sealed class CertificateIntake
{
    private readonly string _inbox;
    private readonly HashSet<string> _acceptedIds = new(StringComparer.Ordinal);
    private readonly List<Certificate> _accepted = new();

    public CertificateIntake(string inbox)
    {
        _inbox = Path.GetFullPath(inbox);
    }

    public IReadOnlyList<Certificate> Accepted => _accepted;

    public void OnCreated(string path)
    {
        if (!File.Exists(path) || !path.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            return;

        var certificate = JsonSerializer.Deserialize<Certificate>(File.ReadAllText(path));
        if (certificate is null ||
            string.IsNullOrWhiteSpace(certificate.CertificateId) ||
            string.IsNullOrWhiteSpace(certificate.DeviceId))
            throw new InvalidDataException("Invalid calibration certificate.");

        if (_acceptedIds.Add(certificate.CertificateId))
            _accepted.Add(certificate);
    }

    public void Reconcile()
    {
        foreach (var path in Directory.EnumerateFiles(_inbox, "*.json", SearchOption.TopDirectoryOnly))
            OnCreated(path);
    }
}
