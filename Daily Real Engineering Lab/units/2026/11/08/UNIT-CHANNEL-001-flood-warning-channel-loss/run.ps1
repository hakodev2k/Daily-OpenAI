$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/ChannelIncident.csproj" --
exit $LASTEXITCODE
