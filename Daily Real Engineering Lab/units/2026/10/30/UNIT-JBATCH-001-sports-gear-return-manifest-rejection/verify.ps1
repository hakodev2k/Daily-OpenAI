$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet run --project (Join-Path $root 'starter/IntakeLab.csproj') -c Release -- --verify
if ($LASTEXITCODE -ne 0) { throw 'Learner starter failed the batch contract.' }
