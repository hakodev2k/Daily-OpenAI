$ErrorActionPreference = 'Stop'

$starter = Join-Path $PSScriptRoot 'starter'
Push-Location $starter
try {
    $output = dotnet run -c Release 2>&1 | Out-String
}
finally {
    Pop-Location
}

Write-Host $output

if ($output -notmatch 'DOWNSTREAM_CALLS=(\d+)') {
    throw 'Không đọc được DOWNSTREAM_CALLS từ learner-editable starter output.'
}

$calls = [int]$Matches[1]

if ($output -notmatch 'RESULTS_CONSISTENT=True') {
    throw 'Regression: business result không còn nhất quán.'
}

if ($calls -gt 2) {
    throw "Fix chưa đạt mục tiêu chống load amplification. DOWNSTREAM_CALLS=$calls; expected <= 2."
}

Write-Host "PASS: business result đúng và downstream load đã được coalesce. DOWNSTREAM_CALLS=$calls"
