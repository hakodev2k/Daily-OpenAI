$ErrorActionPreference = 'Stop'
$unit = Split-Path -Parent $PSScriptRoot
$project = Join-Path $unit 'starter/RedirectLab.csproj'
& dotnet run --project $project -c Release -- --mode verify
exit $LASTEXITCODE
