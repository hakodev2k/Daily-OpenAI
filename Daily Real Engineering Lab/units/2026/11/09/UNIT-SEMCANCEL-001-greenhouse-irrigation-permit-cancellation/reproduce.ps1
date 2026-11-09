$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/IrrigationGate.csproj" -- reproduce
exit $LASTEXITCODE
