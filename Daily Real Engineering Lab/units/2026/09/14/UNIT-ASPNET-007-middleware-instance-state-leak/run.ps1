$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/TenantAuditLab.csproj'
dotnet run --project $project --configuration Release
