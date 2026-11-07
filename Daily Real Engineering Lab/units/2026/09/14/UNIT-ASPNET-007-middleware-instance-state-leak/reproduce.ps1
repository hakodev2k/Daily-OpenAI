$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/TenantAuditLab.csproj'
$output = (dotnet run --project $project --configuration Release) -join "`n"
Write-Host $output
if ($output -notmatch 'request-a -> tenant-b') { throw 'Expected cross-request tenant contamination was not reproduced.' }
if ($output -notmatch 'request-b -> tenant-b') { throw 'Expected request-b audit row was not produced.' }
Write-Host 'Reproduction PASS: starter demonstrates cross-request contamination.'
