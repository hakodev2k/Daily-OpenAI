$ErrorActionPreference='Stop'
$root=Split-Path $PSScriptRoot -Parent
dotnet --version
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet build (Join-Path $root 'starter/ReadinessReplay.csproj') --nologo
exit $LASTEXITCODE
