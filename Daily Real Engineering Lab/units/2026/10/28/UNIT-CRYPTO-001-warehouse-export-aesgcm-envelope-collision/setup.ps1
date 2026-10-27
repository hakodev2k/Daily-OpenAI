$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/EnvelopeIncident.csproj'
dotnet restore $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Restore failed.' }
Write-Host 'SETUP_OK'
