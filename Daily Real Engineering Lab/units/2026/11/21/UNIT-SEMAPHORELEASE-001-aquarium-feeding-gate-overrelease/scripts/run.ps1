$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/../starter/AquariumGate.csproj" --no-restore
exit $LASTEXITCODE
