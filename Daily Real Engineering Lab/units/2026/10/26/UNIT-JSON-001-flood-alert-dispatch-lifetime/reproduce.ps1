$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet run --project './starter/FloodAlertLab.csproj' -- --assert-broken
    if ($LASTEXITCODE -ne 0) { throw 'Original failure not reproduced. Use a fresh starter checkout, or inspect your edits.' }
    Write-Host 'PASS: Original failure reproduced. This does NOT mean the system is fixed.'
}
finally { Pop-Location }
