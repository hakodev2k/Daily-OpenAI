$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/WaterAlerts.csproj'
dotnet run --project $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host 'Compare duplicate/missing IDs above. This command intentionally does not assert a fix.'
