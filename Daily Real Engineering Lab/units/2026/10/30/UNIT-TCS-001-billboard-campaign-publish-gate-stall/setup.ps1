$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/SignalGateLab.csproj'
dotnet --version
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build $project --no-restore --configuration Release
exit $LASTEXITCODE
