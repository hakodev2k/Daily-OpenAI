$project = Join-Path $PSScriptRoot '../starter/ScholarshipLab.csproj'
dotnet run --project $project
exit $LASTEXITCODE
