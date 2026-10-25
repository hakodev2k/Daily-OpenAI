$ErrorActionPreference = "Stop"
Push-Location (Resolve-Path (Join-Path $PSScriptRoot ".."))
try {
    dotnet run --project starter/TelemetryIngest.csproj -- demo
    if ($LASTEXITCODE -ne 0) { throw "demo failed (exit $LASTEXITCODE)" }
}
finally {
    Pop-Location
}
