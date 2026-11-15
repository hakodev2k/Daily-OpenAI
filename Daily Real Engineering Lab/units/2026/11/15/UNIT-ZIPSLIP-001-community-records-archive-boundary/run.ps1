$ErrorActionPreference = 'Stop'
$root = Join-Path $PSScriptRoot 'sandbox'
$project = Join-Path $PSScriptRoot 'starter/ArchiveIntake.csproj'
dotnet run --project $project -- --extract $root
if ($LASTEXITCODE -ne 0) { throw 'Run failed' }
