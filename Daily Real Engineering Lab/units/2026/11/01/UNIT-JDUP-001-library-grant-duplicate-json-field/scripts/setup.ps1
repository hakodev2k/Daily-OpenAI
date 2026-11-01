$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
dotnet restore (Join-Path $root "checks/ContractChecks.csproj")
if ($LASTEXITCODE -ne 0) { throw "dotnet restore failed ($LASTEXITCODE)" }
