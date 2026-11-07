$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '..\starter\OutboundTenantLab.csproj'
dotnet run --project $project --configuration Release -- reproduce
exit $LASTEXITCODE
