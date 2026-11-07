$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PoolNotices.csproj'
dotnet --version
if ($LASTEXITCODE -ne 0) { throw 'dotnet SDK is required.' }
dotnet restore $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Restore failed.' }
dotnet build $project --no-restore --nologo
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
Write-Host 'Starter ready.'
