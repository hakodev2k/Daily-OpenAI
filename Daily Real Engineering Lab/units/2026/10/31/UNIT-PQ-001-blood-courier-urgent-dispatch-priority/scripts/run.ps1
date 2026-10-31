$ErrorActionPreference = 'Stop'
$unit = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
& dotnet run --project (Join-Path $unit 'starter/DispatchLab.csproj') --configuration Release
exit $LASTEXITCODE
