$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/IrrigationGate.csproj" -- verify
exit $LASTEXITCODE
