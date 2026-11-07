$ErrorActionPreference = 'Stop'
dotnet --version
if ($LASTEXITCODE -ne 0) { throw 'Install .NET SDK 8.x first.' }
$project = Join-Path $PSScriptRoot 'starter/PortManifestLab.csproj'
dotnet build $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Starter build failed.' }
Write-Host 'SETUP_COMPLETE'
