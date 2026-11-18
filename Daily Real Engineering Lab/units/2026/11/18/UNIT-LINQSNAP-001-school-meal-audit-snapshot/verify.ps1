$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/SchoolMealAudit.csproj"
if ($LASTEXITCODE -ne 0) { throw 'Verification failed: fix learner-editable starter' }
Write-Host 'Verified: learner starter passes.'
