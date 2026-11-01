$ErrorActionPreference = 'Stop'
$unit = Split-Path -Parent $PSScriptRoot
$project = Join-Path $unit 'starter/RegistryLab.csproj'
& dotnet run --project $project -- reproduce
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host 'PASS: original failure reproduced (not fixed).'
