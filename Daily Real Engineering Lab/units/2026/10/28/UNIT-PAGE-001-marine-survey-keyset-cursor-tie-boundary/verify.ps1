$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/MarineSurveyLab.csproj'
dotnet build $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Learner build failed.' }
$output = & dotnet run --no-build --project $project -- --verify 2>&1
$code = $LASTEXITCODE
$lines = $output -join [Environment]::NewLine
Write-Host $lines
if ($code -ne 0 -or $lines -notmatch 'CHECKS_PASSED=5' -or $lines -notmatch 'VERIFIED:') {
    throw 'Learner implementation failed regression checks.'
}
Write-Host 'VERIFICATION_CONFIRMED'
