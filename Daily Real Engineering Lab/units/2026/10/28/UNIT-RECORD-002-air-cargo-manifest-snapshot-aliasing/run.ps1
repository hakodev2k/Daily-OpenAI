$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet run --project ./starter/ManifestAudit.csproj -- demo
    $result = $LASTEXITCODE
} finally {
    Pop-Location
}
if ($result -ne 0) { throw "Demo failed (exit $result)." }
