$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/NurseryCsv.csproj" -- test
if ($LASTEXITCODE -ne 0) { Write-Error 'VERIFY FAILED: learner starter does not satisfy all contracts'; exit 1 }
Write-Host 'VERIFIED: learner starter passes all contracts'
exit 0
