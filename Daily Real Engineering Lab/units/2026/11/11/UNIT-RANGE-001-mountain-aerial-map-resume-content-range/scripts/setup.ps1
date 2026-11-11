$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/MapResumeLab.csproj'
dotnet restore $project
if ($LASTEXITCODE -ne 0) { throw 'restore failed' }
