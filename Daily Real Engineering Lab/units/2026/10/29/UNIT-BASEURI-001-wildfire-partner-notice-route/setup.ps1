$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $root 'starter/PartnerRouteLab.csproj'
dotnet restore $project
if ($LASTEXITCODE -ne 0) { throw 'dotnet restore failed' }
dotnet build $project -c Release --no-restore
if ($LASTEXITCODE -ne 0) { throw 'dotnet build failed' }
