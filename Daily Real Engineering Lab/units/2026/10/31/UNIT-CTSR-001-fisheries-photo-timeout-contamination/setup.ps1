$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { throw 'Required: .NET SDK 8 or newer.' }
dotnet restore (Join-Path $root 'starter/CoastalInspection.csproj')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build (Join-Path $root 'starter/CoastalInspection.csproj') --no-restore
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host 'SETUP PASS'
