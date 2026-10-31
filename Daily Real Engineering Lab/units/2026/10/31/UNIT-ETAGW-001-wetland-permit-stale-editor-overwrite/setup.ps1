$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/PermitRevisionLab.csproj'
dotnet --version
if ($LASTEXITCODE -ne 0) { throw 'dotnet SDK missing' }
dotnet restore $project
if ($LASTEXITCODE -ne 0) { throw 'restore failed' }
dotnet build $project --no-restore
if ($LASTEXITCODE -ne 0) { throw 'build failed' }
