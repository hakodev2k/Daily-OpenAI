$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/CultureParserLab.csproj -- verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
