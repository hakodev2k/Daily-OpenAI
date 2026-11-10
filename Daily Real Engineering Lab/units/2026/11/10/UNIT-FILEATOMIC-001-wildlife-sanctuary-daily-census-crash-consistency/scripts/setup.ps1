$ErrorActionPreference = 'Stop'
dotnet --version
dotnet build "$PSScriptRoot/../starter/CensusLab.csproj" --configuration Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
