$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ProbeHost.csproj'
dotnet run --project $project -c Release
if ($LASTEXITCODE -ne 0) { throw 'Starter host failed.' }
