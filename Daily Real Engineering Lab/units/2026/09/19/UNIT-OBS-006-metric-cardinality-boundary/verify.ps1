$ErrorActionPreference = 'Stop'
dotnet run --project ./starter/MetricLab.csproj -- verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
