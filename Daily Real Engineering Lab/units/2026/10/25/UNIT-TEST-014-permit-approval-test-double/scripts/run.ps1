$ErrorActionPreference = "Stop"
Push-Location (Resolve-Path (Join-Path $PSScriptRoot ".."))
try {
    dotnet run --project starter/PermitLab.csproj -- observe
    if ($LASTEXITCODE -ne 0) { throw "observe failed (exit $LASTEXITCODE)" }
}
finally { Pop-Location }
