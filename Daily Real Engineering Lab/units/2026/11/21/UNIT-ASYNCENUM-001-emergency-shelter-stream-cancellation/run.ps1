$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/ShelterLab.csproj" -- normal
exit $LASTEXITCODE
