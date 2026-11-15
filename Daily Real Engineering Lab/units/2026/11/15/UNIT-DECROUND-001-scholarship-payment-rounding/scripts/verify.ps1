$project = Join-Path $PSScriptRoot '../starter/ScholarshipLab.csproj'
dotnet run --project $project -- --verify
exit $LASTEXITCODE
