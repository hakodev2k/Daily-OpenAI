$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
& (Join-Path $root 'setup.ps1')
dotnet run --project (Join-Path $root 'starter/PartnerRouteLab.csproj') -c Release --no-build -- reproduce
if ($LASTEXITCODE -ne 0) { throw 'Baseline was not reproduced' }
