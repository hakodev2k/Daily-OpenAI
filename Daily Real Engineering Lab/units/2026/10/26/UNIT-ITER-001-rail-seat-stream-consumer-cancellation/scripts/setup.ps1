$ErrorActionPreference = 'Stop'
$unitRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $unitRoot 'scripts/LabChecks/LabChecks.csproj'
& dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
