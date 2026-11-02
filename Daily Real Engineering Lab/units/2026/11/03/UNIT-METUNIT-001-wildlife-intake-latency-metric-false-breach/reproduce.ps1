$ErrorActionPreference = 'Stop'
Push-Location $PSScriptRoot
try {
    $output = & dotnet run --project 'starter/MetricLab.csproj' 2>&1
    $code = $LASTEXITCODE
    $output | Out-Host
    if ($code -ne 0) { throw 'Starter execution failed.' }
    if (($output -join [Environment]::NewLine) -notmatch '(?m)^BREACH=true\r?$') {
        throw 'Original symptom not reproduced. Was starter already fixed?'
    }
    Write-Host 'REPRODUCED: false SLO breach in starter.'
} finally { Pop-Location }
