$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet run --project ./starter/ManifestAudit.csproj -- verify
    $result = $LASTEXITCODE
} finally {
    Pop-Location
}
if ($result -ne 0) { throw "Verification failed (exit $result). Inspect evidence and edit starter/ManifestService.cs." }
Write-Host "PASS: Learner-edited starter satisfies the manifest contract."
