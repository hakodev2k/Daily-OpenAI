$ErrorActionPreference='Stop'
dotnet run --project ./starter/ForwardedHostLab.csproj -- verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }