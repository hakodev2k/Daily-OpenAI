$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/SlotSwapLab.csproj" -- --verify
exit $LASTEXITCODE
