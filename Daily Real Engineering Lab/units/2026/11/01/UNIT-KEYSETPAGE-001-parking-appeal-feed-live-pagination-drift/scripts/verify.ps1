$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
dotnet run --project (Join-Path $root "checks/ContractChecks.csproj")
if ($LASTEXITCODE -ne 0) { throw "Learner-editable starter failed the paging contract ($LASTEXITCODE)." }
Write-Host "Verification passed against starter/; reference solution was not used."
