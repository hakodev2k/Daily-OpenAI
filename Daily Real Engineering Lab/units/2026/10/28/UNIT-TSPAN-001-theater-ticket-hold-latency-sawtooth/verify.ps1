$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/TicketHoldLab.csproj'
dotnet build $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Learner build failed.' }
$output = & dotnet run --no-build --project $project -- --verify 2>&1
$exitCode = $LASTEXITCODE
$lines = $output -join [Environment]::NewLine
Write-Host $lines
if ($exitCode -ne 0 -or $lines -notmatch 'CHECKS_PASSED=17' -or
    $lines -notmatch 'VERIFIED: learner latency policy satisfies SLA contracts') {
    throw 'Learner implementation did not satisfy SLA regression contracts.'
}
Write-Host 'VERIFICATION_CONFIRMED'
