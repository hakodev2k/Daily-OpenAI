$ErrorActionPreference='Stop'
$project=Join-Path $PSScriptRoot 'starter/DeadlineLab.csproj'
dotnet build $project --nologo | Out-Host
$slow = dotnet run --project $project --no-build 2>&1 | Out-String
Write-Host $slow
if ($slow -notmatch 'RESULT=DEADLINE_EXCEEDED') { throw 'Slow dependency path must stop when the operation deadline is exhausted.' }
if ($slow -notmatch 'ELAPSED_MS=(\d+)') { throw 'Elapsed measurement missing for slow path.' }
$slowElapsed=[int]$Matches[1]
if ($slowElapsed -gt 1150) { throw "Slow path continued too long: ${slowElapsed}ms." }
$fast = dotnet run --project $project --no-build -- --fast 2>&1 | Out-String
Write-Host $fast
if ($fast -notmatch 'RESULT=INVENTORY\+CARRIER') { throw 'Fast dependency path regressed.' }
Write-Host 'VERIFIED: learner code preserves fast-path correctness and enforces the aggregate deadline.'