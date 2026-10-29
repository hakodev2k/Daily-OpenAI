param([ValidateSet('reproduce','verify')][string]$Mode)
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
$listener = New-Object System.Net.Sockets.TcpListener([System.Net.IPAddress]::Loopback, 0)
$listener.Start()
$port = ([System.Net.IPEndPoint]$listener.LocalEndpoint).Port
$listener.Stop()
$base = "http://127.0.0.1:$port"
$trustedOrigin = 'https://portal.insurer.test'
$server = Start-Process -FilePath 'dotnet' -ArgumentList @('starter/bin/Debug/net8.0/ClaimsApi.dll','--urls',$base) -WorkingDirectory $root -PassThru -NoNewWindow
try {
    $ready = $false
    for ($i = 0; $i -lt 100; $i++) {
        if ($server.HasExited) { break }
        try {
            $health = Invoke-WebRequest -Uri "$base/health" -UseBasicParsing -TimeoutSec 2
            if ($health.StatusCode -eq 200) { $ready = $true; break }
        } catch { Start-Sleep -Milliseconds 100 }
    }
    if (-not $ready) { throw 'Local API did not become ready' }
    function Preflight([string]$origin, [string]$method) {
        $h = @{
            Origin = $origin
            'Access-Control-Request-Method' = $method
            'Access-Control-Request-Headers' = 'authorization,content-type,x-client-version'
        }
        $r = Invoke-WebRequest -Uri "$base/api/claims/preview" -Method Options -Headers $h -UseBasicParsing
        return [pscustomobject]@{
            Status = [int]$r.StatusCode
            Origin = [string]$r.Headers['Access-Control-Allow-Origin']
            Methods = [string]$r.Headers['Access-Control-Allow-Methods']
            Headers = [string]$r.Headers['Access-Control-Allow-Headers']
        }
    }
    $a = Preflight $trustedOrigin 'POST'
    $b = Preflight 'https://untrusted.test' 'POST'
    $c = Preflight $trustedOrigin 'DELETE'
    $postHeaders = @{ Origin=$trustedOrigin; Authorization='Bearer synthetic-token'; 'X-Client-Version'='1.8.0' }
    $post = Invoke-WebRequest -Uri "$base/api/claims/preview" -Method Post -Headers $postHeaders -ContentType 'application/json' -Body '{"claimId":"CLM-481"}' -UseBasicParsing
    $direct = $post.StatusCode -eq 200
    $methods = @($a.Methods.Split(',') | ForEach-Object { $_.Trim().ToLowerInvariant() })
    $headers = @($a.Headers.Split(',') | ForEach-Object { $_.Trim().ToLowerInvariant() })
    $trusted = $a.Status -eq 204 -and $a.Origin -eq $trustedOrigin -and
        ($methods -contains 'post') -and ($headers -contains 'authorization') -and
        ($headers -contains 'content-type') -and ($headers -contains 'x-client-version')
    $foreignBlocked = [string]::IsNullOrWhiteSpace($b.Origin)
    $methodBlocked = [string]::IsNullOrWhiteSpace($c.Origin)
    Write-Host "Direct POST HTTP 200: $direct"
    Write-Host "Trusted SPA preflight accepted: $trusted"
    Write-Host "Foreign origin blocked: $foreignBlocked"
    Write-Host "DELETE preflight blocked: $methodBlocked"
    Write-Host "Trusted OPTIONS HTTP $($a.Status), allow-origin='$($a.Origin)', allow-headers='$($a.Headers)'"
    $pass = if ($Mode -eq 'reproduce') { $direct -and (-not $trusted) -and $foreignBlocked -and $methodBlocked }
            else { $direct -and $trusted -and $foreignBlocked -and $methodBlocked }
    if (-not $pass) { throw "$Mode contract assertions failed" }
    Write-Host $(if ($Mode -eq 'reproduce') { 'REPRODUCED: expected starter symptom' } else { 'VERIFIED: positive and negative cases' })
} finally {
    if ($server -and (-not $server.HasExited)) { Stop-Process -Id $server.Id -Force -ErrorAction SilentlyContinue }
}
