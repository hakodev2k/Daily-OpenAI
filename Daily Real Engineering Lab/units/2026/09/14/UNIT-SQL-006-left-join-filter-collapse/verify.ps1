$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/UnitSql006.csproj'
$output = dotnet run --project $project 2>&1
$exitCode = $LASTEXITCODE
$output | ForEach-Object { Write-Host $_ }
if ($exitCode -ne 0) { throw 'Verification failed: learner-editable report still violates the warehouse coverage contract.' }
$text = $output -join "`n"
if ($text -notmatch 'ACTUAL_IDS=1,2,3' -or $text -notmatch 'ROWS=1:25;2:NULL;3:NULL' -or $text -notmatch 'RESULT=PASS') { throw 'Verification failed: expected full warehouse set and optional snapshot values were not observed.' }
Write-Host 'Verification passed: all warehouses are retained and active snapshot data is projected when present.'
