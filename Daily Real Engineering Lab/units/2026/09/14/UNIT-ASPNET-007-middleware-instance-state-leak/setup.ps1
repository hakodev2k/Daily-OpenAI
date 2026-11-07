$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/TenantAuditLab.csproj'
dotnet restore $project
