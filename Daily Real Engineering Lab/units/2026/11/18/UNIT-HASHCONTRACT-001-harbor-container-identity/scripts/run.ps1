$ErrorActionPreference = 'Stop'
$unit = Split-Path $PSScriptRoot -Parent
dotnet run --project (Join-Path $unit 'starter/HarborManifest.csproj')
exit $LASTEXITCODE
