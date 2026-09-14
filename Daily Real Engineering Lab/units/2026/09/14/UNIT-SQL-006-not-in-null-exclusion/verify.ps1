$ErrorActionPreference = 'Stop'

$project = Join-Path $PSScriptRoot 'starter/UnitSql006.csproj'

$output = dotnet run --project $project 2>&1
$exitCode = $LASTEXITCODE
$output | ForEach-Object { Write-Host $_ }

if ($exitCode -ne 0) {
    throw 'Verification failed: learner-editable starter still returns an incorrect eligible-customer set.'
}

$text = $output -join "`n"
if ($text -notmatch 'ACTUAL=1,3,4' -or $text -notmatch 'RESULT=PASS') {
    throw 'Verification failed: expected eligible customers 1,3,4 were not observed.'
}

Write-Host 'Verification passed: correctness and the original regression case are satisfied.'
