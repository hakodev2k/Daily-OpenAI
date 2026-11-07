$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/RedisOutageLab.csproj" -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Verification failed. Keep the authoritative result correct, bound origin fallback, and confirm cache recovery.' }
Write-Host 'Verification passed: learner-editable starter survives the simulated cache outage and resumes cache use after recovery.'
