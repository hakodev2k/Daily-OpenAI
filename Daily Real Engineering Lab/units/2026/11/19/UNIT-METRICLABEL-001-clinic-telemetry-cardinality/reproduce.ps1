$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/ClinicTelemetry.csproj" -- --reproduce
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
