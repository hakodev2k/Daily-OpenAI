$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'starter/TicketHoldLab.csproj'
dotnet --version
if ($LASTEXITCODE -ne 0) { throw 'Missing compatible .NET SDK.' }
dotnet restore $project --nologo
if ($LASTEXITCODE -ne 0) { throw 'Restore failed.' }
Write-Host 'SETUP_OK'
