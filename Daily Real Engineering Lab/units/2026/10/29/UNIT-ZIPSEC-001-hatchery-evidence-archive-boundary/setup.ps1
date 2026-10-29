$ErrorActionPreference = 'Stop'
if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { throw 'Install .NET SDK 8.x first.' }
dotnet restore (Join-Path $PSScriptRoot 'checks/ArchiveChecks.csproj')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host 'Restore completed.'
