$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PharmacyEdi.csproj'
dotnet run --project $project -- verify
if ($LASTEXITCODE -ne 0) { throw 'VERIFY_FAILED: learner implementation still violates wire contract' }
Write-Host 'VERIFY_PASS'
