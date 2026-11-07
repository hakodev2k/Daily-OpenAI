$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/TicketHoldLab.csproj'
dotnet build $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Baseline build failed.' }
$output = & dotnet run --no-build --project $project -- --reproduce 2>&1
$exitCode = $LASTEXITCODE
$lines = $output -join [Environment]::NewLine
Write-Host $lines
if ($exitCode -ne 0 -or $lines -notmatch 'SLOW_1200_FLAGGED=False' -or
    $lines -notmatch 'SLOW_1900_FLAGGED=True' -or
    $lines -notmatch 'SLOW_2500_FLAGGED=False' -or
    $lines -notmatch 'REPRODUCED: latency breach classification oscillates') {
    throw 'Baseline symptom not reproduced.'
}
Write-Host 'REPRODUCTION_CONFIRMED'
