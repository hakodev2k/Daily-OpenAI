param([ValidateSet("reproduce", "verify")][string]$Mode = "reproduce")
$project = Join-Path $PSScriptRoot "harness/Harness.csproj"
dotnet run --project $project -- "--$Mode"
exit $LASTEXITCODE
