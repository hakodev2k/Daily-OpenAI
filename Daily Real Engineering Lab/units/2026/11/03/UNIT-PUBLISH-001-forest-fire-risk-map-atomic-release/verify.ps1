$ErrorActionPreference = 'Stop'
# Validate the learner-editable starter, not the reference solution.
$output = dotnet run --project ./starter/FireMap.csproj
if ($LASTEXITCODE -ne 0) { throw 'Starter build/run failed' }
$output | ForEach-Object { Write-Output $_ }
$checkpoints = @($output | Where-Object { $_ -match '^consistent=' })
if ($checkpoints.Count -ne 2) { throw "Expected exactly 2 consistency observations, got $($checkpoints.Count)" }
if (@($checkpoints | Where-Object { $_ -ne 'consistent=True' }).Count -gt 0) { throw 'A reader observed a mixed release' }
$versions = @($output | Where-Object { $_ -match '^visible=' })
if ($versions.Count -ne 2) { throw 'Expected 2 map snapshots' }
if ($versions[0] -notmatch 'north:r1,south:r1') { throw 'Before publish, old release must remain fully visible' }
if ($versions[1] -notmatch 'north:r2,south:r2') { throw 'After publish, new release must be fully visible' }
Write-Output 'PASS: atomic visible release contract'
