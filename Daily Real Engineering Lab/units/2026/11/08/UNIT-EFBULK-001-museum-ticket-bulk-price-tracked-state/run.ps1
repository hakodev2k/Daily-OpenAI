$ErrorActionPreference = 'Stop'
dotnet run --project "$PSScriptRoot/starter/MuseumLab.csproj" -- run
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
