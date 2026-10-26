$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    dotnet run --project './starter/FloodAlertLab.csproj' -- --assert-fixed
    if ($LASTEXITCODE -ne 0) { throw 'FAIL: Learner starter still violates dispatch contract.' }
    Write-Host 'PASS: Learner-edited starter delivers both alerts without processing failures.'
}
finally { Pop-Location }
