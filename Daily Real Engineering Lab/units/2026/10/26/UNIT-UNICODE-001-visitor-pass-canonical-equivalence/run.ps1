$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/BadgeImport.csproj'
dotnet run --project $project -- --run
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
exit 0
