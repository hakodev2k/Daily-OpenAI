$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/PartitionFanoutLab.csproj" -- --verify
exit $LASTEXITCODE
