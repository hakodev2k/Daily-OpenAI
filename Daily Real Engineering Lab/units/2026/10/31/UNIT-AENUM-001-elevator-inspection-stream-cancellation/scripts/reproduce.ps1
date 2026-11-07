# Exit 0 means the starter reproduced the known failure.
$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/ElevatorScanLab.csproj'
& dotnet run --project $project -- --mode=reproduce
exit $LASTEXITCODE
