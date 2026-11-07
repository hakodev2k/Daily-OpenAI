$project = Join-Path $PSScriptRoot 'starter/UnitCs012.csproj'
dotnet run --project $project -- --reproduce
exit $LASTEXITCODE
