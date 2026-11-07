$ErrorActionPreference = 'Stop'
$root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
& dotnet restore (Join-Path $root 'starter/EvidenceHandoff.csproj')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
