$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PermitAudit.csproj'
$output = & dotnet run --project $project --no-launch-profile 2>&1
$exit = $LASTEXITCODE
$output | ForEach-Object { Write-Output $_ }
$text = $output -join "`n"
if ($exit -eq 0 -and $text -match 'valid-permit:.*PASS' -and $text -match 'missing-permit:.*PASS' -and $text -notmatch 'FAIL') { Write-Output 'VERIFIED: both contracts pass'; exit 0 }
Write-Error 'Verification failed; edit starter/Program.cs, not solution/Program.cs.'
exit 1
