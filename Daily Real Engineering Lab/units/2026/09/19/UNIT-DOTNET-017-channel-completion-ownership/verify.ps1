$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/ChannelLab.csproj -- verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
