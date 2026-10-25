$ErrorActionPreference = 'Stop'
$unitRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $unitRoot 'scripts/LabChecks/LabChecks.csproj'
& dotnet run --project $project -c Release -- --mode observe
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
