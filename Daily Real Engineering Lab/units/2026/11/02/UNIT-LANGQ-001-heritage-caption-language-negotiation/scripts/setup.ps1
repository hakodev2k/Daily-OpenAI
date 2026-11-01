$ErrorActionPreference = 'Stop'
$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnet) { throw 'Missing .NET SDK 8.x. Install SDK from https://dotnet.microsoft.com/download/dotnet/8.0' }
$version = & dotnet --version
if ($LASTEXITCODE -ne 0 -or -not $version.StartsWith('8.')) { throw "Expected .NET SDK 8.x, found $version" }
$unit = Split-Path $PSScriptRoot -Parent
& dotnet restore (Join-Path $unit 'starter/LocaleLab.csproj')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
& dotnet restore (Join-Path $unit 'evidence/baseline/LocaleLab.csproj')
exit $LASTEXITCODE
