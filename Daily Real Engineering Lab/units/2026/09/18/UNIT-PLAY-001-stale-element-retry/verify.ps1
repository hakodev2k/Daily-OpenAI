$ErrorActionPreference='Stop'
dotnet run --project ./starter/FlakyUiLab.csproj -- verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }