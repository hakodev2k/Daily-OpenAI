$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/RiverBatch.csproj" -- --verify
exit $LASTEXITCODE
