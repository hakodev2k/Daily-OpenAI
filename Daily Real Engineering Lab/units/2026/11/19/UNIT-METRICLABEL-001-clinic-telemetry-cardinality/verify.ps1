$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/ClinicTelemetry.csproj" -- --verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
