$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/RadioGateway.csproj'
dotnet run --project $project -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Verification failed for learner-editable starter.' }
