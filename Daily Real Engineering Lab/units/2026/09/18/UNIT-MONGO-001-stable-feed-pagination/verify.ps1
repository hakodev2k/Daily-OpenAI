$ErrorActionPreference='Continue'
$output = dotnet run --project "$PSScriptRoot/starter/StableFeedLab.csproj" 2>&1
$output | Write-Host
if ($LASTEXITCODE -eq 0 -and ($output -join "`n") -match 'TRAVERSAL_OK') { Write-Host 'VERIFY_PASS'; exit 0 }
Write-Error 'VERIFY_FAIL: learner-editable starter still violates the traversal contract.'
exit 1