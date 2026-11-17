$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/WarehouseGate.csproj"
exit $LASTEXITCODE
