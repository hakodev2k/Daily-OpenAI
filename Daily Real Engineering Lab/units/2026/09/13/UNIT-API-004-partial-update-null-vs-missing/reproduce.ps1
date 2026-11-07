$ErrorActionPreference = 'Stop'
& "$PSScriptRoot/setup.ps1"
dotnet run --project "$PSScriptRoot/starter"
if ($LASTEXITCODE -eq 0) { throw 'Starter did not reproduce the intended failure.' }
Write-Host 'Reproduction confirmed: explicit null is not distinguished from a missing property.'
exit 0
