$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    & dotnet run --project "./starter/TelemetryRelay.csproj" -- --reproduce
    if ($LASTEXITCODE -ne 0) { throw "REPRODUCE: dotnet exit code $LASTEXITCODE" }
}
finally { Pop-Location }
