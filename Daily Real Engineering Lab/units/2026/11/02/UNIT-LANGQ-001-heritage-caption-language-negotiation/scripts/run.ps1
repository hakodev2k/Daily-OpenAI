$ErrorActionPreference = 'Stop'
$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if (-not $dotnet) { throw 'Missing .NET SDK 8.x. Install SDK from https://dotnet.microsoft.com/download/dotnet/8.0' }
$unit = Split-Path $PSScriptRoot -Parent
& dotnet run --project (Join-Path $unit 'starter/LocaleLab.csproj') -- inspect
exit $LASTEXITCODE
