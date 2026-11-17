$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
dotnet run --project (Join-Path $root 'starter/ReadinessReplay.csproj') --no-launch-profile -- --verify
exit $LASTEXITCODE
