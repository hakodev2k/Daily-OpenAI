$project = Join-Path $PSScriptRoot 'starter/UnitCs012.csproj'
dotnet run --project $project -- --verify
exit $LASTEXITCODE
