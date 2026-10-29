using CraneInspectionAccess;

var configured = new PermissionGate(new Dictionary<string, Permission>
{
    ["ViewInspection"] = Permission.ViewInspection,
    ["ApproveInspection"] = Permission.ViewInspection | Permission.ApproveInspection,
    ["ExportCertificate"] = Permission.ExportCertificate
});

var incomplete = new PermissionGate(new Dictionary<string, Permission>
{
    ["ViewInspection"] = Permission.ViewInspection,
    ["ApproveInspection"] = Permission.ViewInspection | Permission.ApproveInspection
});

if (args.Length == 1 && args[0] == "--reproduce")
{
    bool normalActionDenied = !configured.CanExecute(
        "ApproveInspection", Permission.ViewInspection);
    bool unexpectedAllow = incomplete.CanExecute(
        "ExportCertificate", Permission.ViewInspection);

    Console.WriteLine($"configured ApproveInspection with view-only: {(normalActionDenied ? "DENY" : "ALLOW")}");
    Console.WriteLine($"unconfigured ExportCertificate with view-only: {(unexpectedAllow ? "ALLOW" : "DENY")}");
    Console.WriteLine(unexpectedAllow
        ? "SYMPTOM REPRODUCED: unconfigured export was allowed."
        : "SYMPTOM NOT REPRODUCED: export was denied.");
    Environment.ExitCode = normalActionDenied && unexpectedAllow ? 0 : 1;
    return;
}

if (args.Length == 1 && args[0] == "--verify")
{
    int failures = 0;

    void Check(string name, bool ok)
    {
        Console.WriteLine($"{(ok ? "PASS" : "FAIL")} {name}");
        if (!ok) failures++;
    }

    Check("view permission allows view",
        configured.CanExecute("ViewInspection", Permission.ViewInspection));
    Check("view-only cannot approve",
        !configured.CanExecute("ApproveInspection", Permission.ViewInspection));
    Check("combined permission can approve",
        configured.CanExecute("ApproveInspection",
            Permission.ViewInspection | Permission.ApproveInspection));
    Check("export permission allows configured export",
        configured.CanExecute("ExportCertificate", Permission.ExportCertificate));
    Check("unmapped export denied to view-only user",
        !incomplete.CanExecute("ExportCertificate", Permission.ViewInspection));
    Check("unmapped export denied to user with no permissions",
        !incomplete.CanExecute("ExportCertificate", Permission.None));
    Check("unknown action denied even to fully privileged user",
        !configured.CanExecute("DeleteArchive",
            Permission.ViewInspection | Permission.ApproveInspection | Permission.ExportCertificate));
    Check("explicit empty requirement denied",
        !new PermissionGate(new Dictionary<string, Permission>
        {
            ["ExportCertificate"] = Permission.None
        }).CanExecute("ExportCertificate", Permission.ExportCertificate));
    Check("unknown required bit denied",
        !new PermissionGate(new Dictionary<string, Permission>
        {
            ["ExportCertificate"] = (Permission)8
        }).CanExecute("ExportCertificate", (Permission)15));

    Console.WriteLine($"Verification: {9 - failures}/9 passed.");
    Environment.ExitCode = failures == 0 ? 0 : 1;
    return;
}

Console.WriteLine("Use --reproduce or --verify.");
Environment.ExitCode = 2;
