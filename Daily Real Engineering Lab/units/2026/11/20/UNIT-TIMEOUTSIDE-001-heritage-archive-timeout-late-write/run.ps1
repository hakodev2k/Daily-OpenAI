$ErrorActionPreference = 'Stop'
$here = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet run --project (Join-Path $here 'starter/ArchiveLab.csproj')
if ($LASTEXITCODE -ne 0) { throw 'dotnet run failed' }
