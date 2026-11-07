$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/Starter.csproj
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }