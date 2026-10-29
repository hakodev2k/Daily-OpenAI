$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet run --project './starter/MeterCodeLab.csproj' -c Release -- verify
    if ($LASTEXITCODE -ne 0) { throw 'Starter has unresolved validation cases; use reproduce.ps1 to confirm the intended failure' }
} finally { Pop-Location }
