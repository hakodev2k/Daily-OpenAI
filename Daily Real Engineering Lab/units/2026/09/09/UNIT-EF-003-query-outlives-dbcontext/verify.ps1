$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Starter.csproj'

dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

dotnet build $project --no-restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$output = & dotnet run --project $project --no-build 2>&1 | Out-String
$exitCode = $LASTEXITCODE
Write-Host $output

if ($exitCode -ne 0) {
    Write-Error "Learner code still fails. Exit code: $exitCode"
    exit 1
}

foreach ($marker in @('RESULT_COUNT=2', 'SKU-LOW-001:2', 'SKU-LOW-002:5')) {
    if ($output -notmatch [regex]::Escape($marker)) {
        Write-Error "Missing expected marker: $marker"
        exit 1
    }
}

if ($output -match 'OBJECT_DISPOSED') {
    Write-Error 'Disposed-context symptom still exists.'
    exit 1
}

Write-Host 'Verification passed: learner-editable starter returns the expected low-stock result.'
exit 0
