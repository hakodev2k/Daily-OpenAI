$ErrorActionPreference='Stop'
dotnet run --project ./starter/InventoryLab.csproj -- verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }