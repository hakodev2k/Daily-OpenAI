$ErrorActionPreference = 'Stop'
dotnet restore ./starter/Lab.csproj
dotnet run --project ./starter/Lab.csproj -- reproduce
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }