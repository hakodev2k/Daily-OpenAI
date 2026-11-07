$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet restore (Join-Path $root 'probe/Probe.csproj')
if ($LASTEXITCODE -ne 0) { throw 'dotnet restore failed' }
dotnet build (Join-Path $root 'probe/Probe.csproj') --no-restore -c Release
if ($LASTEXITCODE -ne 0) { throw 'dotnet build failed' }
