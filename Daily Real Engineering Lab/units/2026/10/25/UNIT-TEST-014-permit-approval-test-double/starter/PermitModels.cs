namespace PermitLab;

public sealed class PermitRecord
{
    public PermitRecord(string permitId, string status, int version)
    {
        PermitId = permitId;
        Status = status;
        Version = version;
    }

    public string PermitId { get; }
    public string Status { get; set; }
    public int Version { get; set; }

    public PermitRecord Snapshot() => new(PermitId, Status, Version);
}
