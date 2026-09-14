$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/Starter.csproj'
dotnet run --project $project -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Expected starter symptom was not reproduced.' }
