$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/PartitionFanoutLab.csproj" -- --reproduce
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
