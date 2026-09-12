$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/CosmosPartitionLab.csproj" -- inspect
dotnet run --project "$PSScriptRoot/starter/CosmosPartitionLab.csproj" -- known