$ErrorActionPreference = "Stop"
$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
& dotnet run --project (Join-Path $root "starter/RoleClaimLab.csproj")
exit $LASTEXITCODE
