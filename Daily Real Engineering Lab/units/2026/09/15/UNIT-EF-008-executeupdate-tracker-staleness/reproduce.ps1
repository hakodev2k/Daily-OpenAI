$project = Join-Path $PSScriptRoot 'starter/UnitEf008.csproj'
dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet run --project $project -- --reproduce
exit $LASTEXITCODE
