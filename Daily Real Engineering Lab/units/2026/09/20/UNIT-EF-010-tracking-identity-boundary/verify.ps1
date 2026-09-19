$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/TrackingIdentityLab.csproj -- verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
