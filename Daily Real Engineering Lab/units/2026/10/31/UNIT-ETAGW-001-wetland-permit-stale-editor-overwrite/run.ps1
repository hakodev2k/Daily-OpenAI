param([ValidateSet('reproduce','verify')][string]$Mode = 'reproduce')
$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PermitRevisionLab.csproj'
dotnet run --project $project -- $Mode
if ($LASTEXITCODE -ne 0) { throw "Starter exit $LASTEXITCODE ($Mode)" }
