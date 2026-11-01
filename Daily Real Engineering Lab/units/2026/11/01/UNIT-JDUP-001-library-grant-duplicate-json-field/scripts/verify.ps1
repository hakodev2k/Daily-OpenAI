$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
dotnet run --project (Join-Path $root "checks/ContractChecks.csproj")
if ($LASTEXITCODE -ne 0) { throw "Learner-editable starter did not satisfy contract ($LASTEXITCODE)" }
Write-Host "Verification passed against starter/ (not the reference solution)."
