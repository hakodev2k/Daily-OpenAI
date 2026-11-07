$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    & dotnet run --project './verification/Verification.csproj' --configuration Release
    if ($LASTEXITCODE -ne 0) { throw 'Verification failed against learner-edited starter/CallbackPolicy.cs.' }
    Write-Host 'VERIFIED: independent callback contract passes.'
} finally { Pop-Location }
