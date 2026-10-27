$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'src/LabHarness/LabHarness.csproj'
dotnet --version
if ($LASTEXITCODE -ne 0) { throw 'dotnet SDK 10 is required.' }
dotnet restore $project
if ($LASTEXITCODE -ne 0) { throw 'Restore failed.' }
dotnet build $project --no-restore
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
Write-Host 'SETUP OK'
