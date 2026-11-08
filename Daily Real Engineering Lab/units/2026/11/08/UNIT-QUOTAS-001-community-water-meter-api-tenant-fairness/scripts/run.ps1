$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/WaterQuota.csproj'
dotnet run --project $project --
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
