$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/GridSignalLab.csproj'
dotnet --version
if ($LASTEXITCODE -ne 0) { throw '.NET SDK 8+ is required.' }
dotnet restore $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Restore failed.' }
Write-Host 'SETUP_COMPLETE'
