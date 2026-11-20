$ErrorActionPreference = 'Stop'
$unit = Split-Path $PSScriptRoot -Parent
dotnet --version
if ($LASTEXITCODE -ne 0) { throw 'Install .NET SDK 8' }
dotnet build (Join-Path $unit 'starter/TenantDispatch.csproj') --nologo
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
