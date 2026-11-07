$ErrorActionPreference = 'Stop'
$out = @(dotnet run --project (Join-Path $PSScriptRoot 'starter/ParcelLocker.csproj'))
if ($LASTEXITCODE -ne 0) { throw 'Learner code execution failed' }
$out | ForEach-Object { Write-Output $_ }
foreach ($expected in @('CALLS 1','CACHE 1','SAME True','FAILURE_RETRY True')) {
  if ($out -notcontains $expected) { throw "Verification failed: missing $expected" }
}
Write-Output 'PASS: one gateway side effect, shared result and recoverable failure.'
