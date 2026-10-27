$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/CaptionPipelineLab.csproj'
dotnet run --project $project -- --run
if ($LASTEXITCODE -ne 0) { throw 'Runner failed.' }
