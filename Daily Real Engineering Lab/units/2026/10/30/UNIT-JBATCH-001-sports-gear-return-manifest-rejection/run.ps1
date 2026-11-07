$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet run --project (Join-Path $root 'starter/IntakeLab.csproj') -c Release -- --demo
if ($LASTEXITCODE -ne 0) { throw 'Demo failed.' }
