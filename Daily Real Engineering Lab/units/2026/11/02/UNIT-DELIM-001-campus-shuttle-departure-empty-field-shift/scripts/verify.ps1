$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/Starter.csproj'
& dotnet run --project $project -- --check
if ($LASTEXITCODE -ne 0) { throw 'Verification failed: fix starter/DepartureParser.cs and rerun.' }
Write-Host 'VERIFIED: learner starter passes all seven behavior contracts.'
