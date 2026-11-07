$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/ElevatorScanLab.csproj'
& dotnet run --project $project -- --mode=run
exit $LASTEXITCODE
