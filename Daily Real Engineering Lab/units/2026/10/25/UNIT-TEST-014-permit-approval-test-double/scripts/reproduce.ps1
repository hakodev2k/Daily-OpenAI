$ErrorActionPreference = "Stop"
Push-Location (Resolve-Path (Join-Path $PSScriptRoot ".."))
try {
    dotnet run --project starter/PermitLab.csproj -- reproduce
    if ($LASTEXITCODE -ne 0) { throw "original discrepancy was not reproduced (exit $LASTEXITCODE)" }
}
finally { Pop-Location }
