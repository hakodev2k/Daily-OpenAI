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
    throw 'Không đọc được DOWNSTREAM_CALLS từ starter output.'
}

$calls = [int]$Matches[1]

if ($output -notmatch 'RESULTS_CONSISTENT=True') {
    throw 'Starter không giữ được business result nhất quán; reproduction không đúng contract.'
}

if ($calls -lt 10) {
    throw "Không reproduce được load amplification đủ rõ. DOWNSTREAM_CALLS=$calls"
}

Write-Host "Reproduced: $calls downstream calls cho một burst request cùng hot key."
