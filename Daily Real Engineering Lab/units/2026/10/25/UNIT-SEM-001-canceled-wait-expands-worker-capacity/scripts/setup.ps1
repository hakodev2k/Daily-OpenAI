$ErrorActionPreference = "Stop"
$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
& dotnet restore (Join-Path $root "checks/ContractChecks.csproj")
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& dotnet build (Join-Path $root "checks/ContractChecks.csproj") --no-restore
exit $LASTEXITCODE
