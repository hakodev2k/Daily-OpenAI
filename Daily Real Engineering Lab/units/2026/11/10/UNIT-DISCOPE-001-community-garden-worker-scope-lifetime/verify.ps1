$ErrorActionPreference='Stop'
dotnet run --project (Join-Path $PSScriptRoot 'starter/Lab.csproj') -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Learner starter code failed verification' }
Write-Host 'VERIFICATION PASSED'
