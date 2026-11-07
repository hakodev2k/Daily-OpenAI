$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/AuditExport.csproj'
dotnet run --project $project -- --reproduce
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
exit 0
