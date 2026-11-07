$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/SeedRegistryLab.csproj'
dotnet build $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Baseline build failed.' }
$output = & dotnet run --no-build --project $project -- --reproduce 2>&1
$code = $LASTEXITCODE
$lines = $output -join [Environment]::NewLine
Write-Host $lines
if ($code -ne 0 -or $lines -notmatch 'REPRODUCED: corrected record remains unreachable' -or
    $lines -notmatch 'FOUND_CORRECTED=False' -or $lines -notmatch 'DUPLICATE_ACCEPTED=True' -or
    $lines -notmatch 'REMAINING_COUNT=1') {
    throw 'Original baseline symptom not reproduced.'
}
Write-Host 'REPRODUCTION_CONFIRMED'
