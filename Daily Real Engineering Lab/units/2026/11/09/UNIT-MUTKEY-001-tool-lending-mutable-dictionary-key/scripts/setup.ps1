$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
dotnet restore (Join-Path $root 'starter/Lab.csproj')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
