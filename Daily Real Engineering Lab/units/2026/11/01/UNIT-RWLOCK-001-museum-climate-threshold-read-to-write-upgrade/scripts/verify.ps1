$ErrorActionPreference = 'Stop'
$unit = Split-Path -Parent $PSScriptRoot
$project = Join-Path $unit 'starter/RegistryLab.csproj'
& dotnet run --project $project -- verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host 'PASS: learner-edited starter verified.'
