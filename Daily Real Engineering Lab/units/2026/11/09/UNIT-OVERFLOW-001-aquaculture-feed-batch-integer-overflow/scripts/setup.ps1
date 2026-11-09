$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/FeedBatch.csproj'
dotnet restore $project
if ($LASTEXITCODE -ne 0) { throw 'setup failed' }
