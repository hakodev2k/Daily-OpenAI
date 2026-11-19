$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/ClinicTelemetry.csproj"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
