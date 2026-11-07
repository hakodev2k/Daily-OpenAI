$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/ProfileConcurrencyLab.csproj -- verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }