$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/ApiPartialUpdateLab.csproj"
if ($LASTEXITCODE -ne 0) { throw 'Verification failed. The learner-editable starter still violates the update contract.' }
Write-Host 'Verification passed.'
