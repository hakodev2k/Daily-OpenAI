$project = Join-Path $PSScriptRoot '../starter/JsonLab.csproj'
& dotnet run --project $project --configuration Release
exit $LASTEXITCODE
