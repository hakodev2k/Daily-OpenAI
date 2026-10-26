$ErrorActionPreference = 'Stop'
$unit = Split-Path -Parent $PSScriptRoot
$project = Join-Path $unit 'starter/RedirectLab.csproj'
& dotnet build $project -c Release
exit $LASTEXITCODE
