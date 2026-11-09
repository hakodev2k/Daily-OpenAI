dotnet run --project (Join-Path $PSScriptRoot 'starter/MarineSampleIntake.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Learner starter contract verification failed' }
Write-Host 'All learner contract cases pass'
exit 0
