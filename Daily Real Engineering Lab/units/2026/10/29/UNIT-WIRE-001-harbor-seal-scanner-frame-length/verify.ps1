$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
& (Join-Path $root 'setup.ps1')
$project = Join-Path $root 'starter/WireFrameLab.csproj'
dotnet run --project $project -c Release --no-build -- verify
if ($LASTEXITCODE -ne 0) { throw 'Learner starter has not passed the wire contract' }
