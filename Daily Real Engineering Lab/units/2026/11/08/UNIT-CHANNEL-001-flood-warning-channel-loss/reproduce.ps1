$ErrorActionPreference = 'Stop'
# Original starter is deterministic; do not modify it before first reproduction.
dotnet run --project "$PSScriptRoot/starter/ChannelIncident.csproj" --
exit $LASTEXITCODE
