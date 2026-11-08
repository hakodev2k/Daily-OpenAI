$ErrorActionPreference = 'Stop'
# Validate learner-edited starter, never the reference implementation.
dotnet run --project "$PSScriptRoot/starter/ChannelIncident.csproj" -- --verify
exit $LASTEXITCODE
