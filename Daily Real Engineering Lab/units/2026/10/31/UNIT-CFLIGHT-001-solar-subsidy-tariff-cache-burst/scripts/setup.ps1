$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
dotnet --version
dotnet restore (Join-Path $root 'starter/Starter.csproj')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet restore (Join-Path $root 'solution/Solution.csproj')
exit $LASTEXITCODE
