$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/GridSignalLab.csproj'
dotnet build $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Baseline build failed.' }
$output = & dotnet run --no-build --project $project -- --reproduce 2>&1
$code = $LASTEXITCODE
$lines = $output -join [Environment]::NewLine
Write-Host $lines
if ($code -ne 0 -or $lines -notmatch 'GATEWAY_REQUESTS=1' -or
    $lines -notmatch 'AUDIT_EVENTS=0' -or
    $lines -notmatch 'OBSERVED_EXCEPTION=InvalidOperationException' -or
    $lines -notmatch 'REPRODUCED: valid signal failed before receipt') {
    throw 'Original baseline symptom not reproduced.'
}
Write-Host 'REPRODUCTION_CONFIRMED'
