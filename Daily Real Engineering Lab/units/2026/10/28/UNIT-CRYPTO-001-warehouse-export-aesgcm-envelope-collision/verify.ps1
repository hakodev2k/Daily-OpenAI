$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/EnvelopeIncident.csproj'
dotnet build $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Learner build failed.' }
$output = & dotnet run --no-build --project $project -- --verify 2>&1
$code = $LASTEXITCODE
$lines = $output -join [Environment]::NewLine
Write-Host $lines
if ($code -ne 0 -or $lines -notmatch 'UNIQUE_ENVELOPES=48' -or
    $lines -notmatch 'VERIFIED:') {
    throw 'Learner implementation did not satisfy the envelope contract.'
}
Write-Host 'VERIFICATION_CONFIRMED'
