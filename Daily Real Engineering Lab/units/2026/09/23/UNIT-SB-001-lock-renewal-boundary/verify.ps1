$ErrorActionPreference='Stop'
$output = dotnet run --project ./starter/Starter.csproj 2>&1
$output | Write-Host
if ($LASTEXITCODE -ne 0) { throw 'Verification failed: learner implementation still violates the delivery/side-effect contract.' }
if (($output -join "`n") -notmatch 'FINAL_SIDE_EFFECTS=1') { throw 'Verification failed: expected exactly one completion side effect.' }
Write-Host 'PASS: learner implementation preserved one completion side effect.'