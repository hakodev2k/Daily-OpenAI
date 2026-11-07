$ErrorActionPreference = "Stop"
dotnet run --project (Join-Path $PSScriptRoot "starter/SlotSwapLab.csproj") --configuration Release -- --reproduce
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
