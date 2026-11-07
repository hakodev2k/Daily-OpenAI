$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '..\starter\ApiConcurrencyLab.csproj'
$output = dotnet run --project $project 2>&1 | Out-String
if ($LASTEXITCODE -ne 0) { throw $output }
if ($output -notmatch '412 Precondition Failed') { throw 'Expected stale write to be rejected with 412 Precondition Failed.' }
if ($output -notmatch 'Da Nang') { throw 'Expected accepted update to remain in final state.' }
Write-Host 'PASS: stale write is rejected and accepted state is preserved.'
