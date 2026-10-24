$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '..\starter\OutboundTenantLab.csproj'
dotnet --version
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build $project --configuration Release --no-restore
exit $LASTEXITCODE
