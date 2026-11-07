# Exit 0 means the learner-edited starter passes.
$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/ElevatorScanLab.csproj'
& dotnet run --project $project -- --mode=verify
exit $LASTEXITCODE
