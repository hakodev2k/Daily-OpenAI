$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/WaterQuota.csproj'
dotnet run --project $project -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Verification failed against learner-edited starter.' }
Write-Output 'PASS: capacity, isolation and bulk service.'
