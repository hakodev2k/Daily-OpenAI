$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ShelterFeed.csproj'
$output = & dotnet run --project $project 2>&1 | Out-String
$code = $LASTEXITCODE
Write-Output $output
if ($code -ne 0) { throw "Starter failed (exit=$code)" }
$expected = @('IMPORTED=3','REVIEW=1','SHELTER=S1;STATUS=Available','SHELTER=S2;STATUS=Unknown','SHELTER=S3;STATUS=Full','RAW=S2:Standby')
foreach ($item in $expected) { if (-not $output.Contains($item)) { throw "Missing contract: $item" } }
if ($output -match 'SHELTER=S2;STATUS=Available') { throw 'Unknown status was treated as available' }
Write-Output 'PASS: learner starter satisfies forward-compatible import contract'
