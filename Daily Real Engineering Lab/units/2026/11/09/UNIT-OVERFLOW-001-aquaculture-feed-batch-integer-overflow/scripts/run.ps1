$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/FeedBatch.csproj'
dotnet run --project $project -- --demo
if ($LASTEXITCODE -ne 0) { throw 'run failed' }
