$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/MarineSurveyLab.csproj'
dotnet --version
if ($LASTEXITCODE -ne 0) { throw 'dotnet SDK is unavailable.' }
dotnet restore $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Restore failed.' }
Write-Host 'SETUP_READY'
