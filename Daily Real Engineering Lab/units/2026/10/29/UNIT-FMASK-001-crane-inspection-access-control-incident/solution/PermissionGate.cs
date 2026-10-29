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
    private const Permission KnownPermissions =
        Permission.ViewInspection | Permission.ApproveInspection | Permission.ExportCertificate;

    private readonly Dictionary<string, Permission> _required;

    public PermissionGate(IDictionary<string, Permission> requirements)
    {
        _required = new Dictionary<string, Permission>(
            requirements, StringComparer.OrdinalIgnoreCase);
    }

    public bool CanExecute(string action, Permission granted)
    {
        if (!_required.TryGetValue(action, out var required))
            return false;

        if (required == Permission.None ||
            (required & ~KnownPermissions) != Permission.None)
            return false;

        return (granted & required) == required;
    }
}
