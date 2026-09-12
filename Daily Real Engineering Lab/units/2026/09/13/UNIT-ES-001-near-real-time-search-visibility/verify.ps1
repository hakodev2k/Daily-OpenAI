$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    docker compose up -d

    for ($i = 0; $i -lt 60; $i++) {
        try {
            $health = Invoke-RestMethod -Uri 'http://localhost:9200/_cluster/health' -TimeoutSec 2
            if ($health.status) { break }
        } catch { }
        Start-Sleep -Seconds 1
    }

    $output = dotnet run --project ./starter/Starter.csproj
    $output | ForEach-Object { Write-Host $_ }

    $writeOk = $output -match 'WRITE_STATUS=20[01]'
    $directOk = $output -match 'DIRECT_GET_STATUS=200'
    $immediateVisible = $output -match 'IMMEDIATE_SEARCH_HITS=1'
    $delayedVisible = $output -match 'DELAYED_SEARCH_HITS=1'

    if (-not ($writeOk -and $directOk -and $immediateVisible -and $delayedVisible)) {
        throw 'VERIFY FAILED: learner-editable starter chưa đáp ứng read-after-write requirement mà không làm mất functional behavior.'
    }

    Write-Host 'VERIFY PASSED: document có thể search ngay trong workflow và vẫn đúng sau đó.'
}
finally {
    Pop-Location
}
