$ErrorActionPreference='Stop'
dotnet run --project ./starter/SearchVisibilityLab.csproj -- verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }