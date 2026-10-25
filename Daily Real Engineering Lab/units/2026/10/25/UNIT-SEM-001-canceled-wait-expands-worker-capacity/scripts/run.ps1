$ErrorActionPreference = "Stop"
$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
& dotnet run --project (Join-Path $root "checks/ContractChecks.csproj") -- demo
exit $LASTEXITCODE
