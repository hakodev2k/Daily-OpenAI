$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/BaggageDispatch.csproj'
dotnet restore $project
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host 'Starter restore complete.'
