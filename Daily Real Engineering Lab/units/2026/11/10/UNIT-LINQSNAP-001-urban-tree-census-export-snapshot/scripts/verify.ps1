$project = Join-Path $PSScriptRoot '../starter/TreeCensus.csproj'
dotnet run --project $project -- --mode verify
exit $LASTEXITCODE
