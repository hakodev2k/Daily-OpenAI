$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PermitAudit.csproj'
$source = Join-Path $PSScriptRoot 'starter/Program.cs'
$original = Join-Path $PSScriptRoot 'solution/../starter/Program.cs'
# Reproduce against starter before editing; script intentionally expects the known failure.
$output = & dotnet run --project $project --no-launch-profile 2>&1
$exit = $LASTEXITCODE
$output | ForEach-Object { Write-Output $_ }
if ($exit -ne 0 -and (($output -join "`n") -match 'valid-permit:.*FAIL')) { Write-Output 'REPRODUCED: valid request rejected'; exit 0 }
Write-Error 'Expected starter regression was not reproduced. Reset starter from Git before reproducing.'
exit 1
