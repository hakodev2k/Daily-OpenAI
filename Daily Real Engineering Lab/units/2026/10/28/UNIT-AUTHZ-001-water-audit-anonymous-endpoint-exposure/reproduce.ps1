$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/AuthBoundary.csproj"
dotnet run --project $project -- --reproduce
exit $LASTEXITCODE
