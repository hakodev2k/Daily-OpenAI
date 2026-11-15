$project = Join-Path $PSScriptRoot '../starter/ScholarshipLab.csproj'
dotnet run --project $project -- --reproduce
exit $LASTEXITCODE
