$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PortManifestLab.csproj'
dotnet build $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Learner build failed.' }
$output = & dotnet run --no-build --project $project -- --verify 2>&1
$code = $LASTEXITCODE
$lines = $output -join [Environment]::NewLine
Write-Host $lines
if ($code -ne 0 -or $lines -notmatch 'CHECKS_PASSED=8' -or
    $lines -notmatch 'VERIFIED:') {
    throw 'Learner implementation failed contract checks.'
}
Write-Host 'VERIFICATION_CONFIRMED'
