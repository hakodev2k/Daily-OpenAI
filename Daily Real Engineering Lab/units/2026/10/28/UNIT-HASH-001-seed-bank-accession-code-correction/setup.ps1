$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/SeedRegistryLab.csproj'
dotnet restore $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Restore failed.' }
dotnet build $project --no-restore --nologo
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
Write-Host 'SETUP_READY'
