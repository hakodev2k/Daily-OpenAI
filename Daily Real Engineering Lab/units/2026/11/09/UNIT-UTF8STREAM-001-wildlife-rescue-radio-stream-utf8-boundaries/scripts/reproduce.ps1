$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/RadioGateway.csproj'
dotnet run --project $project -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Reproduction contract failed: starter symptom not observed.' }
