$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/AuthBoundary.csproj"
dotnet run --project $project -- --serve
exit $LASTEXITCODE
