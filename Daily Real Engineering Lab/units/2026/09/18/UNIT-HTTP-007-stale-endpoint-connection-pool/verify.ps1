$ErrorActionPreference='Stop'
dotnet run --project ./starter/EndpointPoolLab.csproj -- verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }