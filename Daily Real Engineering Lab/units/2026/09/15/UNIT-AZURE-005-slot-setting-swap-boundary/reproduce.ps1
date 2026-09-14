$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/SlotSwapLab.csproj" -- --reproduce
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
