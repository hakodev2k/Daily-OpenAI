$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Starter.csproj'
dotnet run --project $project -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Verification failed. The learner-editable starter still loses work or broke the happy path.' }
Write-Host 'VERIFY_PASS'
