$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
dotnet --version
if ($LASTEXITCODE -ne 0) { throw 'Install .NET 8 SDK' }
dotnet restore (Join-Path $root 'starter/DeviceLab.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Restore failed' }
