$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $root 'starter/WireFrameLab.csproj'
dotnet --version | Out-Null
if ($LASTEXITCODE -ne 0) { throw 'dotnet SDK is required' }
dotnet build $project -c Release
if ($LASTEXITCODE -ne 0) { throw 'Starter build failed' }
