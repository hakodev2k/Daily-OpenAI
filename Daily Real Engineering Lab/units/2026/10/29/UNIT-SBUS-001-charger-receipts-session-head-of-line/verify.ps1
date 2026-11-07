$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
& (Join-Path $root 'setup.ps1')
dotnet run --project (Join-Path $root 'probe/Probe.csproj') --no-build -c Release -- verify
if ($LASTEXITCODE -ne 0) { throw 'Learner workspace does not satisfy behavior contract' }
