$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/SchoolMealAudit.csproj"
if ($LASTEXITCODE -eq 0) { throw 'Expected starter inconsistency' }
Write-Host 'Reproduced: starter exited non-zero.'
