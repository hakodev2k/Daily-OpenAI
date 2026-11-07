$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/Starter.csproj" -- --mode reproduce
if ($LASTEXITCODE -ne 0) { throw 'Starter did not reproduce the expected symptom.' }
Write-Host 'Reproduction confirmed.'