$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
dotnet run --project (Join-Path $root 'starter/IntakeLab.csproj') -c Release -- --reproduce
if ($LASTEXITCODE -ne 0) { throw 'Starter symptom was not reproduced.' }
