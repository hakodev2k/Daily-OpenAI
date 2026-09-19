$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/ArtifactLab.csproj -- verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
