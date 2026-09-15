$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/ProviderFidelity.Tests.csproj'
dotnet restore $project
dotnet test $project --no-restore
if ($LASTEXITCODE -ne 0) { throw 'Starter did not reproduce the intended false-green test state.' }
Write-Host 'Reproduced: duplicate-write scenario is green in the starter test environment.'