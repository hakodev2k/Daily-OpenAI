$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    & dotnet run --project "./starter/TelemetryRelay.csproj"
    if ($LASTEXITCODE -ne 0) { throw "RUN: dotnet exit code $LASTEXITCODE" }
}
finally { Pop-Location }
