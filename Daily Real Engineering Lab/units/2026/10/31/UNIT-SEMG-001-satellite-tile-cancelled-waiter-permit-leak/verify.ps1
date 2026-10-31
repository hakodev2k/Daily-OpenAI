$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/OrbitTileLab.csproj"
dotnet run --project $project -- verify
exit $LASTEXITCODE
