$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/UnitSql006.csproj'
dotnet restore $project
if ($LASTEXITCODE -ne 0) { throw 'dotnet restore failed.' }
dotnet run --project $project --no-restore
$exitCode = $LASTEXITCODE
if ($exitCode -ne 2) { throw "Expected starter to reproduce row loss with exit code 2, but got $exitCode." }
Write-Host 'Reproduced: report completes successfully but omits warehouses required by the business contract.'
exit 0
