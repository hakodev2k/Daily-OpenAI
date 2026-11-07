param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('Reproduce', 'Verify')]
    [string]$Mode
)
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$project = Join-Path $root 'starter/PoolNotices.csproj'
$process = $null
$passed = $false

try {
    & dotnet build $project --nologo --verbosity quiet
    if ($LASTEXITCODE -ne 0) { throw 'Starter build failed.' }

    $dll = Join-Path $root 'starter/bin/Debug/net8.0/PoolNotices.dll'
    if (-not (Test-Path $dll)) { throw 'Starter assembly not found.' }

    $port = Get-Random -Minimum 23000 -Maximum 49000
    $baseUrl = "http://127.0.0.1:$port"
    $process = Start-Process -FilePath 'dotnet' -ArgumentList @("`"$dll`"", '--urls', $baseUrl) -PassThru -NoNewWindow

    $ready = $false
    for ($i = 0; $i -lt 60; $i++) {
        if ($process.HasExited) { throw 'Server exited before becoming ready.' }
        try {
            $health = Invoke-RestMethod -Uri "$baseUrl/health" -TimeoutSec 2
            if ($health.status -eq 'healthy') { $ready = $true; break }
        } catch { Start-Sleep -Milliseconds 200 }
    }
    if (-not $ready) { throw 'Server did not become ready.' }

    $uri = "$baseUrl/api/pool/closure"
    $viHeaders = @{ 'Accept-Language' = 'vi-VN' }
    $enHeaders = @{ 'Accept-Language' = 'en-US' }
    $vi1 = Invoke-RestMethod -Uri $uri -Headers $viHeaders -TimeoutSec 10
    $en1 = Invoke-RestMethod -Uri $uri -Headers $enHeaders -TimeoutSec 10
    $vi2 = Invoke-RestMethod -Uri $uri -Headers $viHeaders -TimeoutSec 10
    $en2 = Invoke-RestMethod -Uri $uri -Headers $enHeaders -TimeoutSec 10

    foreach ($sample in @($vi1, $en1, $vi2, $en2)) {
        Write-Host ("notice={0} language={1} renderCount={2} message={3}" -f $sample.noticeId, $sample.language, $sample.renderCount, $sample.message)
    }

    if ($Mode -eq 'Reproduce') {
        $passed = ($vi1.language -eq 'vi' -and $en1.language -eq 'vi' -and
            $vi1.renderCount -eq $en1.renderCount -and $en1.renderCount -eq $vi2.renderCount)
        if ($passed) { Write-Host 'REPRODUCED: the second language received the first response.' }
    } else {
        $passed = ($vi1.noticeId -eq 'POOL-2026-1030' -and
            $vi1.language -eq 'vi' -and $en1.language -eq 'en' -and
            $vi2.language -eq 'vi' -and $en2.language -eq 'en' -and
            $vi1.renderCount -eq $vi2.renderCount -and
            $en1.renderCount -eq $en2.renderCount -and
            $vi1.renderCount -ne $en1.renderCount -and
            $vi1.message -ne $en1.message)
        if ($passed) { Write-Host 'VERIFIED: correct language and per-variant response reuse.' }
    }
} catch {
    Write-Host ("Probe error: " + $_.Exception.Message)
} finally {
    if ($null -ne $process) {
        try {
            if (-not $process.HasExited) {
                Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
                $process.WaitForExit(5000) | Out-Null
            }
        } catch { }
        $process.Dispose()
    }
}

if ($passed) { exit 0 }
Write-Host "$Mode FAILED."
exit 1
