$project = Join-Path $PSScriptRoot '../starter/TreeCensus.csproj'
dotnet run --project $project -- --mode reproduce
exit $LASTEXITCODE
