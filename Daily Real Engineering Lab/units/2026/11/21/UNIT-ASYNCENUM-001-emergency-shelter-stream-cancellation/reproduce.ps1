$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/ShelterLab.csproj" -- reproduce
exit $LASTEXITCODE
