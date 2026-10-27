$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet --version
if ($LASTEXITCODE -ne 0) { throw 'Required .NET SDK not available.' }
dotnet restore (Join-Path $root 'starter/BulkLab.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Starter restore failed.' }
dotnet restore (Join-Path $root 'solution/BulkLab.Solution.csproj')
if ($LASTEXITCODE -ne 0) { throw 'Solution restore failed.' }
Write-Output 'SETUP_OK'
