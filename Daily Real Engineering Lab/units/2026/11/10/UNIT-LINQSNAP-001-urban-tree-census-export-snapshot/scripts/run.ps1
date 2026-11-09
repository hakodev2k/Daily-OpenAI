$project = Join-Path $PSScriptRoot '../starter/TreeCensus.csproj'
dotnet run --project $project -- run
exit $LASTEXITCODE
