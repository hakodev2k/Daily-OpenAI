$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '..\starter\OrchardFrameLab.csproj'
dotnet restore "$project"
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
