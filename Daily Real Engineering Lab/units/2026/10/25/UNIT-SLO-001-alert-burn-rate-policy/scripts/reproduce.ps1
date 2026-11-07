$ErrorActionPreference = "Stop"
Push-Location (Resolve-Path (Join-Path $PSScriptRoot ".."))
try {
    dotnet run --project starter/SloAlertLab.csproj -- reproduce
    if ($LASTEXITCODE -ne 0) { throw "original discrepancy was not reproduced ($LASTEXITCODE)" }
}
finally { Pop-Location }
