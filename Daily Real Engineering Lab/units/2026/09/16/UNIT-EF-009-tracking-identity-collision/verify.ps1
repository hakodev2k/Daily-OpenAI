$ErrorActionPreference = 'Stop'
$output = dotnet run --project ./starter/Lab.csproj 2>&1
$output | ForEach-Object { Write-Host $_ }
if ($LASTEXITCODE -ne 0) { throw 'Verification failed: learner-editable starter still exits with an error.' }
if (($output -join "`n") -notmatch 'Update completed') { throw 'Verification failed: expected completion marker was not observed.' }
Write-Host 'Verification passed.'