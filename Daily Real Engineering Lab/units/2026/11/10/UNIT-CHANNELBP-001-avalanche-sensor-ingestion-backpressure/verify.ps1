$ErrorActionPreference='Stop'
$project=Join-Path $PSScriptRoot 'starter/Lab.csproj'
dotnet run --project $project --no-launch-profile -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Verification failed: learner workspace did not meet delivery and backpressure invariants' }
Write-Host 'VERIFIED: learner code passed'
