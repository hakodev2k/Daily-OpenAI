$ErrorActionPreference = 'Stop'
$out = @(dotnet run --project (Join-Path $PSScriptRoot 'starter/ParcelLocker.csproj'))
if ($LASTEXITCODE -ne 0) { throw 'Starter execution failed' }
$out | ForEach-Object { Write-Output $_ }
if (($out -notcontains 'CALLS 2') -or ($out -notcontains 'CACHE 1') -or ($out -notcontains 'SAME True') -or ($out -notcontains 'FAILURE_RETRY True')) { throw 'Expected initial concurrency symptom not observed' }
Write-Output 'REPRODUCED: two gateway allocations, one cached result.'
