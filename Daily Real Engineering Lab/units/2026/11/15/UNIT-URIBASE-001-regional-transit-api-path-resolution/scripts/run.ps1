$ErrorActionPreference='Stop'
$root=Split-Path -Parent $PSScriptRoot
dotnet run --project (Join-Path $root 'starter/TransitLab.csproj')
exit $LASTEXITCODE
