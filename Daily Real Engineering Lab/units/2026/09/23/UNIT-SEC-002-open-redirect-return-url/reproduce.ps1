$ErrorActionPreference = 'Stop'
$base = 'http://127.0.0.1:5092'

function Get-Location([string]$returnUrl) {
    try {
        Invoke-WebRequest -Method Post -Uri "$base/login?returnUrl=$([uri]::EscapeDataString($returnUrl))" -MaximumRedirection 0 -ErrorAction Stop | Out-Null
    } catch {
        $response = $_.Exception.Response
        if ($null -eq $response) { throw }
        return $response.Headers.Location.ToString()
    }
}

$local = Get-Location '/profile'
$external = Get-Location 'https://example.com/after-login'

Write-Host "Local destination:    $local"
Write-Host "External destination: $external"

if ($local -ne '/profile') { throw 'Reproduction invalid: local navigation did not behave as expected.' }
if ($external -notmatch '^https?://') { throw 'Starter no longer reproduces the external navigation symptom.' }

Write-Host 'Reproduced: request-controlled navigation can leave the portal after login.'