$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '..\starter\OrchardFrameLab.csproj'
dotnet run --project "$project" -- --reproduce
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
