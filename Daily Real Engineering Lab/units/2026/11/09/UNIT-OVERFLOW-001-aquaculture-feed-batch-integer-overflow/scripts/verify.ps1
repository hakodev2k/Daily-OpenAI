$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/FeedBatch.csproj'
dotnet run --project $project -- --verify
if ($LASTEXITCODE -ne 0) { throw 'verify failed' }
