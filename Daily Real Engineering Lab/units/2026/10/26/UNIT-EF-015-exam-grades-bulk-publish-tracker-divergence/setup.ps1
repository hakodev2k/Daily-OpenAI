$project = Join-Path $PSScriptRoot "harness/Harness.csproj"
dotnet restore $project
exit $LASTEXITCODE
