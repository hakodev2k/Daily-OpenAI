$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/GridSignalLab.csproj'
dotnet build $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Learner build failed.' }
$output = & dotnet run --no-build --project $project -- --verify 2>&1
$code = $LASTEXITCODE
$lines = $output -join [Environment]::NewLine
Write-Host $lines
if ($code -ne 0 -or $lines -notmatch 'RECEIPTS_OK=True' -or
    $lines -notmatch 'AUDIT_OK=True' -or
    $lines -notmatch 'GATEWAY_REQUESTS=2' -or
    $lines -notmatch 'VERIFIED: learner dispatch satisfies all contracts') {
    throw 'Learner implementation failed regression checks.'
}
Write-Host 'VERIFICATION_CONFIRMED'
