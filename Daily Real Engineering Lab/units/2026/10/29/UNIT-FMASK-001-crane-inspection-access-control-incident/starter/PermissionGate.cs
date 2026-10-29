namespace CraneInspectionAccess;

[Flags]
public enum Permission
{
    None = 0,
    ViewInspection = 1,
    ApproveInspection = 2,
    ExportCertificate = 4
}

public sealed class PermissionGate
{
    private readonly Dictionary<string, Permission> _required;

    public PermissionGate(IDictionary<string, Permission> requirements)
    {
        _required = new Dictionary<string, Permission>(
            requirements, StringComparer.OrdinalIgnoreCase);
    }

    public bool CanExecute(string action, Permission granted)
    {
        var required = _required.TryGetValue(action, out var configured)
            ? configured
            : Permission.None;

        return granted.HasFlag(required);
    }
}
