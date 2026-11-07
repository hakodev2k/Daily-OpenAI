$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $root
try {
    docker compose up -d

    Write-Host 'Waiting for Elasticsearch...'
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
    $immediateZero = $output -match 'IMMEDIATE_SEARCH_HITS=0'
    $delayedOne = $output -match 'DELAYED_SEARCH_HITS=1'

    if (-not ($writeOk -and $directOk -and $immediateZero -and $delayedOne)) {
        throw 'Không reproduce được expected starter symptom. Chạy docker compose down -v rồi thử lại.'
    }

    Write-Host 'REPRODUCED: write thành công, direct GET thấy document, immediate search chưa thấy, delayed search thấy.'
}
finally {
    Pop-Location
}
