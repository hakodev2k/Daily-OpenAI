$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/TenantAuditLab.csproj'
$output = (dotnet run --project $project --configuration Release) -join "`n"
Write-Host $output
if ($output -notmatch 'request-a -> tenant-a') { throw 'request-a is not isolated to tenant-a.' }
if ($output -notmatch 'request-b -> tenant-b') { throw 'request-b is not isolated to tenant-b.' }
if ($output -match 'request-a -> tenant-b|request-b -> tenant-a') { throw 'Cross-request contamination still exists.' }
Write-Host 'Verification PASS: request-specific tenant state is isolated.'
