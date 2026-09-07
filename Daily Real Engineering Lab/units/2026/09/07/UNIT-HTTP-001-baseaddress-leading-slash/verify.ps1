$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "starter/Lab.csproj"
dotnet run --project $project -- --verify
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
