$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/ClinicChannel.csproj'
& dotnet run --project $project --no-launch-profile -- --verify
exit $LASTEXITCODE
