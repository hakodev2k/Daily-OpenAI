$ErrorActionPreference = 'Stop'
dotnet restore (Join-Path $PSScriptRoot 'starter/QueueLab.Starter.csproj')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
dotnet restore (Join-Path $PSScriptRoot 'solution/QueueLab.Solution.csproj')
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
Write-Host 'Dependencies restored.'
