$ErrorActionPreference = "Stop"
$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
& dotnet restore (Join-Path $root "checks/ContractChecks.csproj")
exit $LASTEXITCODE
