$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/CaptionPipelineLab.csproj'
dotnet --version
if ($LASTEXITCODE -ne 0) { throw 'Install .NET SDK 8 and add dotnet to PATH.' }
dotnet restore $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Restore failed.' }
dotnet build $project --no-restore --nologo
if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }
Write-Host 'SETUP_COMPLETE'
