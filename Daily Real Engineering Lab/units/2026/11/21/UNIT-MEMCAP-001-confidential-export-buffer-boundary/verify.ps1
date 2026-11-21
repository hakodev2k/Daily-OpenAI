$ErrorActionPreference = "Stop"
dotnet run --project "$PSScriptRoot/starter/ExportProbe.csproj" -- --check
if ($LASTEXITCODE -ne 0) { throw "Verification failed: learner starter still violates byte-integrity contract." }
Write-Host "VERIFIED: learner-edited starter preserves exact payload bytes."
