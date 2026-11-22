$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot '../starter/LibraryHoldLab.csproj'
dotnet restore $project
if ($LASTEXITCODE -ne 0) { throw 'Restore failed' }
dotnet build $project --no-restore
if ($LASTEXITCODE -ne 0) { throw 'Build failed' }
