$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet run --project './starter/BadgeDrainLab.csproj' -c Release -- reproduce
    if ($LASTEXITCODE -ne 0) { throw 'Starter symptom was not reproduced' }
} finally { Pop-Location }
