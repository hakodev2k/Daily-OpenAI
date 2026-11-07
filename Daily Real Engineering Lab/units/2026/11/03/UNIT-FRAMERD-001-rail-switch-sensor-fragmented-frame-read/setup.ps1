$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/FrameLab.csproj'
dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build $project --no-restore
exit $LASTEXITCODE
