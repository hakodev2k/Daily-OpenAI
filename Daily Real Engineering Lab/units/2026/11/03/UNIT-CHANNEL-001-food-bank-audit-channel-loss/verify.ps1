$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/AuditLab.csproj" -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Verification failed: audit sequence incomplete' }
Write-Host 'PASS: learner starter audit is complete'
