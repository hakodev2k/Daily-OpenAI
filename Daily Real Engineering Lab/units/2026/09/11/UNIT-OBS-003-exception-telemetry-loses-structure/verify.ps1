$ErrorActionPreference = 'Stop'
dotnet build ./starter/Lab.csproj --no-restore
dotnet run --project ./starter/Lab.csproj --no-build -- verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }