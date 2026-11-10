$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/../starter/CensusLab.csproj" --configuration Release
exit $LASTEXITCODE
