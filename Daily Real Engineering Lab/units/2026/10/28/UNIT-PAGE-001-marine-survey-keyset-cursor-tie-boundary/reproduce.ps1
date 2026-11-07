$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/MarineSurveyLab.csproj'
dotnet build $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Build failed; reproduction invalid.' }
$output = & dotnet run --no-build --project $project -- --reproduce 2>&1
$code = $LASTEXITCODE
$lines = $output -join [Environment]::NewLine
Write-Host $lines
if ($code -ne 0 -or $lines -notmatch 'REPRODUCED: missing=obs-c' -or $lines -notmatch 'SOURCE_COUNT=6' -or $lines -notmatch 'RECEIVED_COUNT=5') {
    throw 'Original baseline symptom not reproduced.'
}
Write-Host 'REPRODUCTION_CONFIRMED'
