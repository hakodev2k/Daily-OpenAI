$ErrorActionPreference = 'Stop'
$unit = Split-Path -Parent $PSScriptRoot
dotnet --version
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet restore (Join-Path $unit 'harness/Contracts.csproj')
exit $LASTEXITCODE
