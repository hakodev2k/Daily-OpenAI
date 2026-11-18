$ErrorActionPreference='Stop'
dotnet run --project "$PSScriptRoot/starter/ClockLab.csproj" -- --check
if ($LASTEXITCODE -eq 0) { throw 'Starter unexpectedly passed' }
Write-Host 'Expected failure reproduced'
