$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/ShelterLab.csproj" -- verify
exit $LASTEXITCODE
