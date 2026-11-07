$ErrorActionPreference = 'Stop'
$unit = Split-Path -Parent $PSScriptRoot
$project = Join-Path $unit 'starter/RegistryLab.csproj'
& dotnet run --project $project -- demo
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
