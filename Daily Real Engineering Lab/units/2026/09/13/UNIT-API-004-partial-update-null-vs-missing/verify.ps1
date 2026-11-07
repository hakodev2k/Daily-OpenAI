$ErrorActionPreference = 'Stop'
& "$PSScriptRoot/setup.ps1"
dotnet run --project "$PSScriptRoot/starter"
if ($LASTEXITCODE -ne 0) { throw 'Verification failed. The learner-editable starter still violates the PATCH contract.' }
Write-Host 'Verification passed.'
