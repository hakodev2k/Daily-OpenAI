$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    & dotnet run --project "./starter/TelemetryRelay.csproj" -- --verify
    if ($LASTEXITCODE -ne 0) { throw "VERIFY: dotnet exit code $LASTEXITCODE" }
}
finally { Pop-Location }
