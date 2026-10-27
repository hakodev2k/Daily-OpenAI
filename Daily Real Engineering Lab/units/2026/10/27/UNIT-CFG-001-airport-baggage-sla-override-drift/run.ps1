$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/BaggageSlaLab.csproj'
dotnet run --project $project
if ($LASTEXITCODE -ne 0) { throw 'Starter failed to run.' }
